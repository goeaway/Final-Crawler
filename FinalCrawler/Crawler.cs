using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FinalCrawler.Abstractions;
using FinalCrawler.Abstractions.Data;
using FinalCrawler.Abstractions.Factories;
using FinalCrawler.Abstractions.Web;
using FinalCrawler.Core;
using FinalCrawler.Core.Abstractions;
using FinalCrawler.Core.Pausing;
using FinalCrawler.Core.StopConditions;
using FinalCrawler.Data;
using FinalCrawler.Factories;
using FinalCrawler.Web;
using PuppeteerSharp;

namespace FinalCrawler
{
    public class Crawler
    {
        private const int BLOOM_FILTER_MAX = 10_000_000;
        private const int DEFAULT_MAX_CRAWL = 1_000_000;
        private TimeSpan DEFAULT_MAX_CRAWL_TIME = TimeSpan.FromDays(7);
        private TimeSpan DEFAULT_CRAWL_NO_PROGRESS_LIMIT = TimeSpan.FromSeconds(10);

        private readonly IBrowserFactory _browserFactory;
        private readonly IDataExtractor _dataExtractor;
        private readonly IDataProcessor _dataProcessor;
        private readonly IRateLimiter _rateLimiter;
        private readonly IRobotParser _robotParser;
        private readonly INowProvider _nowProvider;

        private IBloomFilter _crawledFilter;
        private readonly ConcurrentQueue<Uri> _queue;
        private readonly ConcurrentBag<Uri> _crawled;
        private readonly Stopwatch _stopwatch;

        private uint _threads = 1;

        /// <summary>
        /// Gets or sets the amount of threads this crawler should use when crawling
        /// </summary>
        public uint Threads
        {
            get => _threads;
            set
            {
                if (value < 1 || value > 64)
                {
                    throw new ArgumentOutOfRangeException("Crawler must have at least 1 thread and no more than 64 threads");
                }

                _threads = value;
            }
        }

        public Crawler(IDataProcessor dataProcessor, INowProvider nowProvider) : this (
            dataProcessor, 
            nowProvider,
            new DefaultBrowserFactory(), 
            new DataExtractor(), 
            new RollingWindowRateLimiter(TimeSpan.FromSeconds(30), 100, nowProvider), 
            new RobotParser(new HttpClient())) { }

        public Crawler(
            IDataProcessor dataProcessor, 
            INowProvider nowProvider,
            IBrowserFactory browserFactory, 
            IDataExtractor dataExtractor, 
            IRateLimiter rateLimiter,
            IRobotParser robotParser)
        {
            _browserFactory = browserFactory ?? throw new ArgumentNullException(nameof(browserFactory));
            _dataExtractor = dataExtractor ?? throw new ArgumentNullException(nameof(dataExtractor));
            _dataProcessor = dataProcessor ?? throw new ArgumentNullException(nameof(dataProcessor));
            _rateLimiter = rateLimiter ?? throw new ArgumentNullException(nameof(rateLimiter));
            _robotParser = robotParser ?? throw new ArgumentNullException(nameof(robotParser));
            _nowProvider = nowProvider ?? throw new ArgumentNullException(nameof(nowProvider));

            _queue = new ConcurrentQueue<Uri>();
            _crawled = new ConcurrentBag<Uri>();
            _stopwatch = new Stopwatch();
        }

        public async Task<CrawlReport> Crawl(Job job, CancellationToken cancellationToken, PauseToken pauseToken)
        {
            _crawledFilter = new RichardKundlBloomFilter(BLOOM_FILTER_MAX);

            _queue.Clear();
            foreach (var uri in job.Seeds)
            {
                _queue.Enqueue(uri);
            }

            _crawled.Clear();
            _stopwatch.Restart();

            _dataExtractor.LoadCustomRegexPattern(job.DataPattern);

            using (var browser = await _browserFactory.GetBrowser())
            {
                var userAgent = await browser.GetUserAgentAsync();
                var stopConditions = CreateStopConditions(job);

                var tasks = Enumerable
                    .Range(0, (int)Threads)
                    .Select(i => ThreadWork(
                            job,
                            stopConditions,
                            browser,
                            userAgent,
                            cancellationToken,
                            pauseToken));

                await Task.WhenAll(tasks);

                _stopwatch.Stop();
                return GetCrawlReport();
            }
        }

        private async Task ThreadWork(
            Job job, 
            IEnumerable<ICrawlStopCondition> stopConditions,
            Browser browser, 
            string userAgent,
            CancellationToken cancellationToken, PauseToken pauseToken)
        {
            using (var page = await browser.NewPageAsync())
            {
                while (!cancellationToken.IsCancellationRequested && stopConditions.All(sc => !sc.ShouldStop(GetCrawlReport())))
                {
                    if (pauseToken.IsPaused)
                    {
                        await pauseToken.WaitWhilePausedAsync();
                    }
                    
                    // get the next item from the queue
                    var next = GetNext();

                    // if we didn't dequeue anything, try again in a minute
                    // if this one has already been done, skip it (we check when adding it as well below, but there's a chance it could have been crawled already by the time we get back to here with it)
                    // we check for non containment to avoid false positives, we can know for sure if something is NOT in the filter
                    // but it could potentially think something is in the filter when it's not
                    if (next != null && !_crawledFilter.Contains(next.AbsolutePath))
                    {
                        // wait if required by the rate limiter (per domain rate limit)
                        //await _rateLimiter.HoldIfRequired(next);
                        
                        // access the page
                        var response = await page.GoToAsync(next.ToString());
                        _crawled.Add(next);
                        _crawledFilter.Add(next.AbsolutePath);

                        if (!response.Ok)
                        {
                            // create a log of big errors?
                            continue;
                        }

                        // retrieve the page's content
                        var content = await page.GetContentAsync();
                        if (job.QueueNewLinks)
                        {
                            var primedNextAbsolutePath = !next.AbsolutePath.EndsWith("/")
                                ? next.AbsolutePath + "/"
                                : next.AbsolutePath;

                            foreach (var link in _dataExtractor.ExtractUris(next, content))
                            {
                                // todo use bloom filter for crawled, span the beginning of the queue
                                // must be from the same place as the crawled link, must not have been crawled already or in the queue, must be allowed to crawl the page by the robots.txt parser
                                if (link.Host == next.Host 
                                    && link.AbsolutePath.Contains(primedNextAbsolutePath) 
                                    && !_crawledFilter.Contains(link.AbsolutePath)
                                    && !await _robotParser.UriForbidden(link, userAgent))
                                {
                                    _queue.Enqueue(link);
                                }
                            }
                        }
                        // get the data out 
                        var data = _dataExtractor.ExtractData(content);
                        // pass the data off to the processor, with the uri as a source
                        await _dataProcessor.ProcessData(next, data);
                    }
                }
            }
        }

        private Uri GetNext()
        {
            var success = _queue.TryDequeue(out Uri next);
            return next;
        }

        private CrawlReport GetCrawlReport()
        {
            return new CrawlReport
            {
                Crawled =  _crawled,
                DataExtractionCount = _dataProcessor.ProcessCount,
                TimeElapsed = _stopwatch.Elapsed
            };
        }

        private IEnumerable<ICrawlStopCondition> CreateStopConditions(Job job)
        {
            return new List<ICrawlStopCondition>
            {
                new MaxTimeStopCondition(DEFAULT_MAX_CRAWL_TIME),
                new MaxCrawlAmountStopCondition(DEFAULT_MAX_CRAWL),
                new CrawledProgressTimeoutStopCondition(DEFAULT_CRAWL_NO_PROGRESS_LIMIT, _nowProvider)
            };
        }
    }
}

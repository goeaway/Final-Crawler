using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FinalCrawler.Abstractions;
using FinalCrawler.Abstractions.Data;
using FinalCrawler.Abstractions.Factories;
using FinalCrawler.Abstractions.Web;
using FinalCrawler.Core;
using FinalCrawler.Core.Pausing;
using FinalCrawler.Data;
using FinalCrawler.Factories;
using FinalCrawler.Web;
using PuppeteerSharp;

namespace FinalCrawler
{
    public class Crawler
    {
        private readonly IBrowserFactory _browserFactory;
        private readonly IDataExtractor _dataExtractor;
        private readonly IDataProcessor _dataProcessor;
        private readonly IRateLimiter _rateLimiter;
        private readonly IRobotParser _robotParser;

        private readonly ConcurrentQueue<Uri> _queue;
        private readonly ConcurrentBag<Uri> _crawled;

        private int _threads = 1;

        /// <summary>
        /// Gets or sets the amount of threads this crawler should use when crawling
        /// </summary>
        public int Threads
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

        public Crawler(IDataProcessor dataProcessor) : this (dataProcessor, new DefaultBrowserFactory(), new DataExtractor(), new RollingWindowRateLimiter(TimeSpan.FromSeconds(30), 100, new NowProvider()), new RobotParser()) { }

        public Crawler(
            IDataProcessor dataProcessor, 
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

            _queue = new ConcurrentQueue<Uri>();
            _crawled = new ConcurrentBag<Uri>();
        }

        public async Task<CrawlReport> Crawl(Job job, CancellationToken cancellationToken, PauseToken pauseToken)
        {
            _queue.Clear();

            foreach (var uri in job.Seeds)
            {
                _queue.Enqueue(uri);
            }

            _crawled.Clear();

            using (var browser = await _browserFactory.GetBrowser())
            {
                var handles = new List<ManualResetEvent>();
                var threads = new List<Thread>();
                for (var i = 0; i < Threads; i++)
                {
                    var handle = new ManualResetEvent(false);
                    handles.Add(handle);
                    var thread = new Thread(async () => await ThreadWork(browser, job, handle, cancellationToken, pauseToken));
                    threads.Add(thread);
                    thread.Start();
                }

                WaitHandle.WaitAll(handles.ToArray());

                foreach (var thread in threads)
                {
                    thread.Join();
                }

                return new CrawlReport
                {

                };
            }
        }

        private async Task ThreadWork(Browser browser, Job job, ManualResetEvent resetEvent, CancellationToken cancellationToken, PauseToken pauseToken)
        {
            using (var page = await browser.NewPageAsync())
            {
                do
                {
                    if (pauseToken.IsPaused)
                    {
                        await pauseToken.WaitWhilePausedAsync();
                    }
                    
                    // get the next item from the queue
                    var next = GetNext();

                    // if we didn't dequeue anything or the robots.txt for the domain denies access...
                    if (next == null)
                    {
                        continue;
                    }

                    // wait if required by the rate limiter (per domain rate limit)
                    //await _rateLimiter.HoldIfRequired(next);

                    // access the page
                    var response = await page.GoToAsync(next.ToString());
                    _crawled.Add(next);

                    if (!response.Ok)
                    {
                        // create a log of big errors?
                        continue;
                    }

                    // retrieve the page's content
                    var content = await page.GetContentAsync();
                    if (job.QueueNewLinks)
                    {
                        var newLinks = _dataExtractor.ExtractUris(content);

                        foreach (var link in newLinks)
                        {
                            // todo use bloom filter for crawled, span the beginning of the queue
                            if (!_crawled.Contains(link) && !_queue.Contains(link) && !await _robotParser.UriForbidden(link))
                            {
                                _queue.Enqueue(link);
                            }
                        }
                    }
                    // get the data out 
                    var data = _dataExtractor.ExtractData(content, job.DataPattern);
                    // pass the data off to the processor, with the uri as a source
                    await _dataProcessor.ProcessData(next, data);
                } while (!cancellationToken.IsCancellationRequested && job.StopConditions.All(sc => !sc.ShouldStop(GetCrawlReport())));

                resetEvent.Set();
            }
        }

        private Uri GetNext()
        {
            var success = _queue.TryDequeue(out Uri next);
            return next;
        }

        private CrawlReport GetCrawlReport()
        {
            return null;
        }
    }
}

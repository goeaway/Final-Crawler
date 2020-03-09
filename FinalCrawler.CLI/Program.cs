using System;
using System.Collections.Generic;
using System.Threading;
using CommandLine;
using FinalCrawler.Core;
using FinalCrawler.Core.Abstractions;
using FinalCrawler.Core.Pausing;
using FinalCrawler.Core.StopConditions;
using FinalCrawler.Data;

namespace FinalCrawler.CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(o =>
                {
                    var dataProcessor = new BatchOffloadDataProcessor();

                    var crawler = new Crawler(dataProcessor)
                    {
                        Threads = o.Threads
                    };

                    var job = new Job
                    {
                        Seeds = new List<Uri>
                        {
                            new Uri("https://google.com")
                        },
                        StopConditions = new List<ICrawlStopCondition>
                        {
                            new MaxTimeStopCondition(TimeSpan.FromSeconds(20))
                        }
                    };

                    var cancelSource = new CancellationTokenSource();
                    var pauseSource = new PauseTokenSource();

                    var result = crawler.Crawl(job, cancelSource.Token, pauseSource.Token).ConfigureAwait(false).GetAwaiter().GetResult();

                    Console.WriteLine(result.Crawled);

                });
        }
    }
}

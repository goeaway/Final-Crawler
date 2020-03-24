using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
using FinalCrawler.Core;
using FinalCrawler.Core.Pausing;
using FinalCrawler.Data;

namespace FinalCrawler.CLI
{
    public class Program
    {
        private readonly CancellationTokenSource _cancelSource;

        static void Main(string[] args)
        {
            var program = new Program();

            program.Start(args);
        }

        public Program()
        {
            _cancelSource = new CancellationTokenSource();
            Console.CancelKeyPress += ConsoleBreakPressedHandler;
        }

        public void Start(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(async(o) =>
                {
                    if(string.IsNullOrWhiteSpace(o.Path) || (!File.Exists(o.Path) && !Directory.Exists(o.Path)))
                    {
                        throw new ArgumentException("path is required and must be an existing file or directory");
                    }

                    var jobLoader = new JobLoader();
                    var dataProcessor = new BatchOffloadDataProcessor();
                    var crawler = new Crawler(dataProcessor, new NowProvider())
                    {
                        Threads = o.Threads
                    };

                    // if the path is to a directory we'll treat that directory as our queue, and keep checking the directory for files
                    // to use as jobs, we never stop until the app is forced to stop by Ctrl + C/Break
                    // files are deleted once processing is complete
                    if (File.GetAttributes(o.Path).HasFlag(FileAttributes.Directory))
                    {
                        while (true)
                        {
                            if (Console.KeyAvailable)
                            {
                                var key = Console.ReadKey(true).Key;
                                if (key == ConsoleKey.Q)
                                {
                                    Console.WriteLine("Are you sure you want to quit? [Y/N]");
                                    var confirmKey = Console.ReadKey().Key;
                                    if (confirmKey == ConsoleKey.Y)
                                    {
                                        break;
                                    }
                                }
                            }

                            var nextJob = await jobLoader.LoadJob(o.Path);

                            if (nextJob.Item2 != null)
                            {
                                await CrawlWork(crawler, nextJob.Item2);
                                await jobLoader.DeleteJob(nextJob.Item1);
                            }
                            else
                            {
                                await Task.Delay(10);
                            }
                        }
                    }
                    // if the path is just a single file, we run that file and then stop execution
                    else
                    {
                        var job = await jobLoader.LoadJob(o.Path);
                        await CrawlWork(crawler, job.Item2);
                        await jobLoader.DeleteJob(job.Item1);
                    }
                });
        }

        private async Task CrawlWork(Crawler crawler, Job job)
        {
            var pauseSource = new PauseTokenSource();

            var crawlTask = crawler.Crawl(job, _cancelSource.Token, pauseSource.Token);

            while (!crawlTask.IsCanceled && !crawlTask.IsCompleted && !_cancelSource.IsCancellationRequested)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true).Key;
                    //if (key == ConsoleKey.P)
                    //{
                    //    pauseSource.IsPaused = !pauseSource.IsPaused;
                    // https://stackoverflow.com/a/21712588
                    //    Console.WriteLine($"Processing {(pauseSource.IsPaused ? "paused" : "resumed")}");
                    //}
                }

                Thread.Sleep(10);
            }

            var result = await crawlTask;
        }

        private void ConsoleBreakPressedHandler(object sender, ConsoleCancelEventArgs args)
        {
            _cancelSource.Cancel();
        }
    }
}

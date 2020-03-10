using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using FinalCrawler.Abstractions.Data;
using FinalCrawler.Core;
using FinalCrawler.Core.Abstractions;
using FinalCrawler.Core.Pausing;
using FinalCrawler.Core.StopConditions;
using FinalCrawler.Data;
using FinalCrawler.Factories;
using Moq;

namespace FinalCrawler.Tests
{
    [TestClass]
    public class CrawlerTests
    {
        [TestMethod]
        public async Task Crawl_CanCrawlASetOfUris()
        {
            var dataProcessorMock = new Mock<IDataProcessor>();

            var crawler = new Crawler(dataProcessorMock.Object);

            var cancelSource = new CancellationTokenSource();
            var pauseSource = new PauseTokenSource();

            var job = new Job
            {
                QueueNewLinks = true,
                Seeds = new List<Uri>
                {
                    new Uri("https://localhost:44306")
                },
                StopConditions = new List<ICrawlStopCondition>
                {
                    new MaxCrawlAmountStopCondition(30),
                    new MaxTimeStopCondition(TimeSpan.FromSeconds(10))
                }
            };

            var result = await crawler.Crawl(job, cancelSource.Token, pauseSource.Token);

            Assert.AreEqual(30, result.Crawled.Count());
        }
    }
}

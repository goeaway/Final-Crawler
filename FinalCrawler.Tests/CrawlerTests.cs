using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using FinalCrawler.Abstractions.Data;
using FinalCrawler.Abstractions.Web;
using FinalCrawler.Core;
using FinalCrawler.Core.Abstractions;
using FinalCrawler.Core.Pausing;
using FinalCrawler.Core.StopConditions;
using FinalCrawler.Data;
using FinalCrawler.Factories;
using Moq;
using FinalCrawler.Abstractions.Factories;

namespace FinalCrawler.Tests
{
    [TestClass]
    [TestCategory("Crawler Tests")]
    public class CrawlerTests
    {
        private (Mock<IBrowserFactory>, Mock<IDataProcessor>, Mock<IDataExtractor>, Mock<IRateLimiter>, Mock<IRobotParser>) GetMocks()
            => (
                new Mock<IBrowserFactory>(),
                new Mock<IDataProcessor>(), 
                new Mock<IDataExtractor>(), 
                new Mock<IRateLimiter>(),
                new Mock<IRobotParser>());

        private (CancellationTokenSource, PauseTokenSource) GetTokenSources()
            => (
                new CancellationTokenSource(),
                new PauseTokenSource()
                );

        [TestMethod]
        public async Task Crawl_Can_Be_Cancelled()
        {
            var (mockBF, mockDP, mockDE, mockRL, mockRP) = GetMocks();
            var crawler = new Crawler(mockDP.Object, new NowProvider(), new DefaultBrowserFactory(), mockDE.Object, mockRL.Object, mockRP.Object);
            var (cSource, pSource) = GetTokenSources();
            var job = new Job
            {
                Seeds = new List<Uri>
                {
                    new Uri("http://localhost")
                }
            };

            var task = crawler.Crawl(job, cSource.Token, pSource.Token);

            Assert.IsFalse(task.IsCompleted);
            Assert.IsFalse(task.IsCanceled);

            cSource.Cancel();

            await task;


        }

        [TestMethod]
        public async Task Crawl_Can_Be_Paused()
        {
            var (mockBF, mockDP, mockDE, mockRL, mockRP) = GetMocks();
            var crawler = new Crawler(mockDP.Object, new NowProvider(), mockBF.Object, mockDE.Object, mockRL.Object, mockRP.Object);
            var (cSource, pSource) = GetTokenSources();
            var job = new Job
            {
                Seeds = new List<Uri>
                {
                    new Uri("http://domain.com")
                }
            };

            var task = crawler.Crawl(job, cSource.Token, pSource.Token);

            Assert.IsFalse(task.IsCompleted);
            Assert.IsFalse(task.IsCanceled);

            pSource.IsPaused = true;

            await task;
        }
    }
}

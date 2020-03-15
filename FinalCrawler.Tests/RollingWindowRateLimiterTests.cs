using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using FinalCrawler.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FinalCrawler.Tests
{
    [TestClass]
    [TestCategory("Rolling Window Rate Limiter")]
    public class RollingWindowRateLimiterTests
    {
        [TestMethod]
        public void Throws_If_maxRequest_Parameter_Is_Zero()
        {
            Assert
                .ThrowsException<ArgumentOutOfRangeException>(
                    () => new RollingWindowRateLimiter(
                        TimeSpan.MinValue,
                        0,
                        new NowProvider()));
        }

        [TestMethod]
        public void Throws_If_maxRequest_Parameter_Is_Less_Than_Zero()
        {
            Assert
                .ThrowsException<ArgumentOutOfRangeException>(
                    () => new RollingWindowRateLimiter(
                        TimeSpan.MinValue,
                        -1,
                        new NowProvider()));
        }

        [TestMethod]
        public void Throws_If_NowProvider_Null()
        {
            Assert
                .ThrowsException<ArgumentNullException>(
                    () => new RollingWindowRateLimiter(
                        TimeSpan.MinValue,
                        1,
                        null));
        }

        [TestMethod]
        public async Task Does_Not_Hold_If_Domain_New()
        {
            var window = TimeSpan.MaxValue;
            var nowProvider = new NowProvider(new DateTime(2020, 01, 01));
            var limiter = new RollingWindowRateLimiter(window, 1, nowProvider);

            var uri = new Uri("http://domain.com/something");

            var waitTask = limiter.HoldIfRequired(uri);
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            while (!waitTask.IsCompleted)
            {
                if (stopwatch.ElapsedMilliseconds > 1000)
                {
                    Assert.Fail("Took too long");
                }
            }

            await waitTask;
        }

        [TestMethod]
        public async Task Holds_For_All_URIs_With_Same_Domain()
        {
            var window = TimeSpan.FromMilliseconds(1000);
            var nowProvider = new NowProvider(new DateTime(2020, 01, 01));
            var limiter = new RollingWindowRateLimiter(window, 1, nowProvider);

            var firstURI = new Uri("http://domain.com/something");
            var secondURI = new Uri("http://domain.com/something-else");

            var stopwatch = new Stopwatch();
            await limiter.HoldIfRequired(firstURI);
            stopwatch.Start();
            await limiter.HoldIfRequired(secondURI);
            Assert.IsTrue(stopwatch.ElapsedMilliseconds >= 1000);
        }

        [TestMethod]
        [DataRow(1, 1)]
        public async Task Holds_Until_Oldest_Passed_Window_If_Max_Reached(int windowSeconds, int max)
        {
        }
    }
}

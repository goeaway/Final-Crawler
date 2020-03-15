using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FinalCrawler.Tests
{
    [TestClass]
    public class RollingWindowRateLimiterTests
    {
        [TestMethod]
        public void Throws_If_maxRequest_Parameter_Is_Less_Than_One()
        {

        }

        [TestMethod]
        public void Throws_If_NowProvider_Null()
        {

        }

        [TestMethod]
        public async Task Does_Not_Hold_If_Domain_New()
        {

        }

        [TestMethod]
        [DataRow(2, 100)]
        public async Task Holds_Until_Oldest_Passed_Window_If_Max_Reached(int windowSeconds, int max)
        {

        }
    }
}

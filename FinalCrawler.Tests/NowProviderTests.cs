using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FinalCrawler.Tests
{
    [TestClass]
    [TestCategory("Now Provider")]
    public class NowProviderTests
    {
        [TestMethod]
        public async Task Return_DateTime_Now_If_No_Value_Set()
        {
            var provider = new NowProvider();

            var firstGet = provider.Now;
            await Task.Delay(10);
            var secondGet = provider.Now;

            Assert.AreNotEqual(firstGet, secondGet);
            Assert.AreNotEqual(DateTime.MinValue, firstGet);
        }

        [TestMethod]
        public void Return_Value_Set()
        {
            var provider = new NowProvider(new DateTime(2020, 01, 01));
            Assert.AreEqual(new DateTime(2020, 01, 01), provider.Now);
        }
    }
}

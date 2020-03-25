using System;
using System.Collections.Generic;
using System.Text;
using FinalCrawler.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FinalCrawler.Tests
{
    [TestClass]
    [TestCategory("Bloom Filter")]
    public class BloomFilterTests
    {
        [TestMethod]
        [DataRow(1_000)]
        [DataRow(100_000)]
        [DataRow(1_000_000)]
        [DataRow(10_000_000)]
        public void PossiblyExists_Returns_False_For_Not_In(int size)
        {
            const string IN_FILTER = "i'm in the filter already";
            const string NOT_IN_FILTER = "i'm not in the filter";

            var filter = new RichardKundlBloomFilter(size);

            filter.Add(IN_FILTER);

            var result = filter.Contains(NOT_IN_FILTER);

            Assert.IsFalse(result);
        }

        [TestMethod]
        [DataRow(1_000)]
        [DataRow(100_000)]
        [DataRow(1_000_000)]
        [DataRow(10_000_000)]
        public void PossiblyExists_Returns_True_For_In(int size)
        {
            const string IN_FILTER = "i'm in the filter already";

            var filter = new RichardKundlBloomFilter(size);

            filter.Add(IN_FILTER);

            var result = filter.Contains(IN_FILTER);

            Assert.IsTrue(result);
        }
    }
}

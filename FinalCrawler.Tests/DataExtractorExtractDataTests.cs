using FinalCrawler.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FinalCrawler.Tests
{
    [TestClass]
    [TestCategory("Data Extractor - Extract Data")]
    public class DataExtractorExtractDataTests
    {
        [TestMethod]
        public void ExtractData_Returns_Empty_Collection_If_HTML_Null()
        {
            var extractor = new DataExtractor();

            var extracted = extractor.ExtractData(null);

            Assert.AreEqual(0, extracted.Count());
        }

        [TestMethod]
        public void ExtractData_Throws_If_ExtractData_Called_Before_Regex_Set()
        {
            const string HTML = "matching text";
            var extractor = new DataExtractor();
            var extracted = extractor.ExtractData(HTML);

            Assert.ThrowsException<InvalidOperationException>(() => extracted.Count());
        }

        [TestMethod]
        public void ExtractData_Returns_Collection_Of_Matching_Data()
        {
            const string HTML = "<span>matching text</span><div matching text></div>";

            var extractor = new DataExtractor();
            extractor.LoadCustomRegexPattern("matching text");

            var extracted = extractor.ExtractData(HTML);

            Assert.AreEqual(2, extracted.Count());
            Assert.AreEqual("matching text", extracted.First());
            Assert.AreEqual("matching text", extracted.Last());
        }
    }
}

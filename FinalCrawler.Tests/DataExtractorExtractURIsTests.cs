using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FinalCrawler.Data;

namespace FinalCrawler.Tests
{
    [TestClass]
    [TestCategory("Data Extractor - Extract URIs")]
    public class DataExtractorExtractURIsTests
    {
        [TestMethod]
        public void ExtractUris_Extracts_Multiple_URIs_From_Single_String()
        {
            var source = new Uri("https://source.com");

            const string html = "<a href=\"link-to-follow\">link</a><a href=\"link-to-follow-again\">link</a>";

            var extractor = new DataExtractor();

            var extracted = extractor.ExtractUris(source, html);

            Assert.AreEqual(2, extracted.Count());
            Assert.AreEqual("https://source.com/link-to-follow/", extracted.First().ToString());
            Assert.AreEqual("https://source.com/link-to-follow-again/", extracted.Last().ToString());
        }

        [TestMethod]
        public void ExtractUris_Extracts_URI_From_Href_Using_Speech_Marks()
        {
            var source = new Uri("https://source.com");

            const string html = "<a href=\"link-to-follow\">link</a>";

            var extractor = new DataExtractor();

            var extracted = extractor.ExtractUris(source, html);

            Assert.AreEqual(1, extracted.Count());
            Assert.AreEqual("https://source.com/link-to-follow/", extracted.First().ToString());
        }

        [TestMethod]
        public void ExtractUris_Extracts_URI_From_Href_Using_Apostrophes()
        {
            var source = new Uri("https://source.com");

            const string html = "<a href='link-to-follow'>link</a>";

            var extractor = new DataExtractor();

            var extracted = extractor.ExtractUris(source, html);

            Assert.AreEqual("https://source.com/link-to-follow/", extracted.First().ToString());
        }

        [TestMethod]
        public void ExtractUris_Adds_Source_Scheme_To_Site_URIs()
        {
            var source = new Uri("http://source.com");

            const string html = "<a href=\"link-to-follow\">a</a>";

            var extractor = new DataExtractor();

            var extracted = extractor.ExtractUris(source, html);

            Assert.IsTrue(extracted.First().ToString().Contains("http://"));
        }

        [TestMethod]
        public void ExtractUris_Adds_Source_Host_To_Site_URIs()
        {
            var source = new Uri("http://source.com");

            const string html = "<a href=\"link-to-follow\">a</a>";

            var extractor = new DataExtractor();

            var extracted = extractor.ExtractUris(source, html);

            Assert.IsTrue(extracted.First().ToString().Contains("source.com"));
        }

        [TestMethod]
        public void ExtractUris_Adds_Source_Port_To_Site_URIs()
        {
            var source = new Uri("http://source.com:1");

            const string html = "<a href=\"link-to-follow\">a</a>";

            var extractor = new DataExtractor();

            var extracted = extractor.ExtractUris(source, html);

            Assert.IsTrue(extracted.First().ToString().Contains(":1"));
        }

        [TestMethod]
        public void ExtractUris_Adds_Trailing_Slash_To_Site_URIs()
        {
            var source = new Uri("http://source.com");

            const string html = "<a href=\"link-to-follow\">a</a>";

            var extractor = new DataExtractor();

            var extracted = extractor.ExtractUris(source, html);

            Assert.IsTrue(extracted.First().ToString().EndsWith("/"));
        }

        [TestMethod]
        public void ExtractUris_Does_Not_Add_Slash_To_Site_URIs_That_Already_Have_One()
        {
            var source = new Uri("http://source.com");

            const string html = "<a href=\"link-to-follow/\">a</a>";

            var extractor = new DataExtractor();

            var extracted = extractor.ExtractUris(source, html);

            Assert.IsFalse(extracted.First().ToString().EndsWith("//"));
        }

        [TestMethod]
        public void ExtractUris_Does_Not_Alter_External_Links()
        {
            var source = new Uri("http://source.com");

            const string html = "<a href=\"https://other-site.com\">a</a>";

            var extractor = new DataExtractor();

            var extracted = extractor.ExtractUris(source, html);

            Assert.IsFalse(extracted.First().ToString().Contains("http://source.com"));
        }

        [TestMethod]
        public void ExtractUris_Adds_Slash_Between_Host_And_Path()
        {
            var source = new Uri("http://source.com");

            const string html = "<a href=\"link-to-follow\">a</a>";

            var extractor = new DataExtractor();

            var extracted = extractor.ExtractUris(source, html);

            Assert.IsTrue(extracted.First().ToString().Contains("source.com/link-to-follow"));
        }

        [TestMethod]
        public void ExtractUris_Does_Not_Add_Slash_Between_Host_And_Path_If_Already_There()
        {
            var source = new Uri("http://source.com");

            const string html = "<a href=\"/link-to-follow\">a</a>";

            var extractor = new DataExtractor();

            var extracted = extractor.ExtractUris(source, html);

            Assert.IsFalse(extracted.First().ToString().Contains("source.com//link-to-follow"));
        }

        [TestMethod]
        public void ExtractUris_Only_Extracts_URIs_From_A_Tags()
        {
            var source = new Uri("http://source.com");

            const string html = "<link href=\"/link-to-follow\">";

            var extractor = new DataExtractor();

            var extracted = extractor.ExtractUris(source, html);

            Assert.AreEqual(0, extracted.Count());
        }
    }
}

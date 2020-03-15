using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FinalCrawler.Web;
using Moq;

namespace FinalCrawler.Tests
{
    [TestClass]
    [TestCategory("Robot Parser")]
    public class RobotParserTests
    {
        [TestMethod]
        public async Task Makes_Request_To_URIs_Host_Robots_File_If_Nothing_Found_In_Cache()
        {
            var mockClient = new Mock<HttpClient>();
            var parser = new RobotParser(mockClient.Object);
            var testURI = new Uri("http://uri.com");
            const string USER_AGENT = "USER AGENT";

            await parser.UriForbidden(testURI, USER_AGENT);

            mockClient.Verify(m => m.GetStringAsync("http://uri.com:80/robots.txt"));
        }

        [TestMethod]
        public async Task Disallows_URIs_That_Are_Relevant_To_All_User_Agents()
        {

        }

        [TestMethod]
        public async Task Allows_URIs_That_Match_A_Fobidden_Path_For_A_User_Agent_That_Is_Not_The_Crawlers()
        {

        }

        [TestMethod]
        public async Task Disallows_URIs_That_Are_In_A_Forbidden_Path()
        {

        }

        [TestMethod]
        public async Task Disallows_URIs_That_Match_Forbidden_Path_With_Wildcard()
        {

        }

        [TestMethod]
        public async Task Allows_URIs_That_Do_Not_Match_Forbidden_Paths()
        {
            var mockClient = new Mock<HttpClient>();
            var parser = new RobotParser(mockClient.Object);
            var testUri = new Uri("http://source.com/definitely-not-forbidden");
            const string USER_AGENT = "USER AGENT";

            var result = await parser.UriForbidden(testUri, USER_AGENT);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Throws_Exception_If_Http_Client_Is_Null()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new RobotParser(null));
        }
    }
}

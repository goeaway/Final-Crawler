using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FinalCrawler.Web;
using Moq;
using Moq.Protected;

namespace FinalCrawler.Tests
{
    [TestClass]
    [TestCategory("Robot Parser")]
    public class RobotParserTests
    {
        private (HttpClient, Mock<HttpMessageHandler>) GetMockableClient()
        {
            var mock = new Mock<HttpMessageHandler>();
            return (new HttpClient(mock.Object), mock);
        }

        [TestMethod]
        public async Task Makes_Request_To_URIs_Host_Robots_File_If_Nothing_Found_In_Cache()
        {
            var (client, mockMessageHandler) = GetMockableClient();
            var parser = new RobotParser(client);
            var testUri = new Uri("http://source.com/forbidden-path/resource.html");
            const string USER_AGENT = "USER AGENT";

            mockMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(@"
                        User-Agent: user agent
                        Disallow: /forbidden-path/
                    ")
                });

            await parser.UriForbidden(testUri, USER_AGENT);

            mockMessageHandler
                .Protected()
                .Verify(
                    "SendAsync",
                    Times.Once(),
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>());
        }

        [TestMethod]
        public async Task Disallows_URIs_That_Are_Relevant_To_All_User_Agents()
        {
            var (client, mockMessageHandler) = GetMockableClient();
            var parser = new RobotParser(client);
            var testUri = new Uri("http://source.com/forbidden-path/resource.html");
            const string USER_AGENT = "USER AGENT";

            mockMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(@"
                        User-Agent: *
                        Disallow: /forbidden-path/
                    ")
                });

            var result = await parser.UriForbidden(testUri, USER_AGENT);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task Allows_URIs_That_Match_A_Fobidden_Path_For_A_User_Agent_That_Is_Not_The_Crawlers()
        {
            var (client, mockMessageHandler) = GetMockableClient();
            var parser = new RobotParser(client);
            var testUri = new Uri("http://source.com/forbidden-for-some/not-for-others");
            const string USER_AGENT = "USER AGENT";

            mockMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(@"
                        User-Agent: some user agent
                        Disallow: /forbidden-for-some/
                    ")
                });

            var result = await parser.UriForbidden(testUri, USER_AGENT);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task Parses_All_Disallows_For_A_User_Agent()
        {
            var (client, mockMessageHandler) = GetMockableClient();
            var parser = new RobotParser(client);
            var testUri = new Uri("http://source.com/forbidden-path-two/resource.html");
            const string USER_AGENT = "USER AGENT";

            mockMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(@"
                        User-Agent: user agent
                        Disallow: /forbidden-path-one/
                        Disallow: /forbidden-path-two/
                    ")
                });

            var result = await parser.UriForbidden(testUri, USER_AGENT);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task Disallows_URIs_That_Match_Forbidden_Path_With_Wildcard()
        {
            var (client, mockMessageHandler) = GetMockableClient();
            var parser = new RobotParser(client);
            var testUri = new Uri("http://source.com/forbidden-path/resource.html?sort=new");
            const string USER_AGENT = "USER AGENT";

            mockMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(@"
                        User-Agent: user agent
                        Disallow: /forbidden-path/*sort=new
                    ")
                });

            var result = await parser.UriForbidden(testUri, USER_AGENT);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task Allows_URIs_That_Match_All_Of_Forbidden_Path_But_Wildcard()
        {
            var (client, mockMessageHandler) = GetMockableClient();
            var parser = new RobotParser(client);
            var testUri = new Uri("http://source.com/forbidden-path/resource.html");
            const string USER_AGENT = "USER AGENT";

            mockMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(@"
                        User-Agent: user agent
                        Disallow: /forbidden-path/*sort=new
                    ")
                });

            var result = await parser.UriForbidden(testUri, USER_AGENT);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task Allows_URIs_That_Do_Not_Match_Forbidden_Paths()
        {
            var (client, mockMessageHandler) = GetMockableClient();
            var parser = new RobotParser(client);
            var testUri = new Uri("http://source.com/definitely-not-forbidden/something");
            const string USER_AGENT = "USER AGENT";

            mockMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(@"
                        User-Agent: user agent
                        Disallow: /forbidden-path/
                    ")
                });

            var result = await parser.UriForbidden(testUri, USER_AGENT);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Throws_Exception_If_Http_Client_Is_Null()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new RobotParser(null));
        }

        [TestMethod]
        public async Task Handles_A_Null_User_Agent()
        {
            var (client, mockMessageHandler) = GetMockableClient();
            var parser = new RobotParser(client);
            var testUri = new Uri("http://source.com/forbidden-path/something");

            mockMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(@"
                        User-Agent: *
                        Disallow: /forbidden-path/
                    ")
                });

            var result = await parser.UriForbidden(testUri, null);

            Assert.IsTrue(result);
        }
    }
}

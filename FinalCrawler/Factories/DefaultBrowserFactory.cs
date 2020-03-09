using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FinalCrawler.Abstractions.Factories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using PuppeteerSharp;

namespace FinalCrawler.Factories
{
    public class DefaultBrowserFactory : IBrowserFactory
    {
        private readonly ILoggerFactory _loggerFactory;

        public DefaultBrowserFactory()
        {
            _loggerFactory = new NullLoggerFactory();
        }

        public DefaultBrowserFactory(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory ?? new NullLoggerFactory();
        }

        public async Task<Browser> GetBrowser()
        {
            await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);

            return await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true,
                Args = new []
                {
                    "--no-sandbox",
                    "--disable-setuid-sandbox"
                }
            }, _loggerFactory);
        }
    }
}

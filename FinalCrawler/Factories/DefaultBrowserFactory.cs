using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FinalCrawler.Abstractions.Factories;
using PuppeteerSharp;

namespace FinalCrawler.Factories
{
    public class DefaultBrowserFactory : IBrowserFactory
    {
        public async Task<Browser> GetBrowser()
        {
            await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);
            return await Puppeteer.LaunchAsync(new LaunchOptions
            {
                //Headless = true
            });
        }
    }
}

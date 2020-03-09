using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PuppeteerSharp;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision).ConfigureAwait(false).GetAwaiter().GetResult();

            var browser = Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true
            }).ConfigureAwait(false).GetAwaiter().GetResult();

            var page = browser.NewPageAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        }
    }
}

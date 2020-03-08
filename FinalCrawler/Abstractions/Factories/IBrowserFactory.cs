using PuppeteerSharp;
using System.Threading.Tasks;

namespace FinalCrawler.Abstractions.Factories
{
    public interface IBrowserFactory
    {
        Task<Browser> GetBrowser();
    }
}
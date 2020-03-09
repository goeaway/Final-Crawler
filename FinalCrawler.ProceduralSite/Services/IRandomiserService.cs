using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalCrawler.ProceduralSite.Services
{
    public interface IRandomiserService
    {
        IEnumerable<string> GetRandomUrls(int seed);
        IEnumerable<string> GetRandomEmails(int seed);
        IEnumerable<string> GetRandomImages(int seed);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalCrawler.ProceduralSite.Services
{
    public interface IRandomiserService
    {
        int GetRandomWait();
        IEnumerable<string> GetRandomUrls(string basePath);
        IEnumerable<string> GetRandomEmails();
        IEnumerable<string> GetRandomImages();
    }
}

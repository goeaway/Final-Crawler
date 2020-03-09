using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalCrawler.ProceduralSite.Services.Impl
{
    public class RandomiserService : IRandomiserService
    {
        public IEnumerable<string> GetRandomEmails(int seed)
        {
            var random = new Random(seed);

            yield return random.Next() + "e@e" + random.Next() + ".com";
        }

        public IEnumerable<string> GetRandomImages(int seed)
        {
            var possibles = new string[] { };
            var random = new Random(seed);

            yield return "/images/image-" + possibles[random.Next(0, possibles.Length - 1)] + ".jpg";
        }

        public IEnumerable<string> GetRandomUrls(int seed)
        {
            var random = new Random(seed);

            yield return "/" + Guid.NewGuid() + "?seed=" + seed;
        }
    }
}

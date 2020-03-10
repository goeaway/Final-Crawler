using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalCrawler.ProceduralSite.Services.Impl
{
    public class RandomiserService : IRandomiserService
    {
        public int GetRandomWait()
        {
            var random = new Random();

            return random.Next(0, 1000);
        }

        public IEnumerable<string> GetRandomEmails()
        {
            var random = new Random();

            for (var i = 0; i < random.Next(2, 300); i++)
            {
                yield return random.Next() + "e@e" + random.Next() + ".com";
            }
        }

        public IEnumerable<string> GetRandomImages()
        {
            var random = new Random();
            var range = Enumerable.Range(0, 15).ToList();

            for (var i = 0; i < random.Next(5, 500); i++)
            {
                yield return "/images/" + range[random.Next(0, range.Count() - 1)] + ".jpg";
            }
        }

        public IEnumerable<string> GetRandomUrls(string basePath)
        {
            var random = new Random();

            for (var i = 0; i < random.Next(10, 1000); i++)
            {
                // 66% chance the url will have the basePath in it
                yield return (random.Next(0, 3) != 1 ? basePath : "") + (basePath != "/" ? "/" : "") + Guid.NewGuid();
            }
        }
    }
}

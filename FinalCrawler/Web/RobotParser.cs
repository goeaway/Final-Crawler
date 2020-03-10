using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FinalCrawler.Abstractions.Web;

namespace FinalCrawler.Web
{
    public class RobotParser : IRobotParser
    {
        private readonly IWebAgent _webAgent;
        private readonly ConcurrentDictionary<string, IEnumerable<string>> _forbiddenPaths;

        public RobotParser(IWebAgent webAgent)
        {
            _webAgent = webAgent ?? throw new ArgumentNullException(nameof(webAgent));
            _forbiddenPaths = new ConcurrentDictionary<string, IEnumerable<string>>();
        }

        public async Task<bool> UriForbidden(Uri uri)
        {
            if (!_forbiddenPaths.TryGetValue(uri.Host, out IEnumerable<string> forbidden))
            {
                // make request to get the forbidden urls

                // update below so we can use it in the next block
                forbidden = new List<string>();
                _forbiddenPaths.TryAdd(uri.Host, forbidden);
            }

            // Disallow: / answers / accounts /
            // Disallow: / answers / users /
            // Disallow: / answers / revisions /
            // Disallow: / answers / search
            // Disallow: / answers/*sort=newest
            // Disallow: /answers/*sort=hottest
            // Disallow: /answers/*sort=votes <----??????
            // Disallow: /answers/commands/
            // Disallow: /answers/badges/
            // Disallow: /answers/comments

            var testUrl = uri.AbsolutePath;
            foreach (var path in forbidden)
            {
                if (testUrl.Length >= path.Length && testUrl.StartsWith(path))
                {
                    return true;
                }
            }
        }
    }
}

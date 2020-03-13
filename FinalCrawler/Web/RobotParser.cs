using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FinalCrawler.Abstractions.Web;

namespace FinalCrawler.Web
{
    public class RobotParser : IRobotParser
    {
        private readonly HttpClient _client;
        private readonly ConcurrentDictionary<string, IEnumerable<string>> _forbiddenPaths;

        public RobotParser(HttpClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _forbiddenPaths = new ConcurrentDictionary<string, IEnumerable<string>>();
        }

        public async Task<bool> UriForbidden(Uri uri, string userAgent)
        {
            if (!_forbiddenPaths.TryGetValue(uri.Host, out IEnumerable<string> forbidden))
            {
                // make request to get the forbidden urls
                var result = await _client
                    .GetStringAsync(
                        uri.Scheme + "://" + uri.Host + ":" + uri.Port + "/robots.txt");

                if (result != null)
                {
                    forbidden = ParseRobotsFile(result, userAgent);
                    _forbiddenPaths.TryAdd(uri.Host, forbidden);
                }
            }

            var testUrl = uri.AbsolutePath;
            foreach (var path in forbidden)
            {
                // if path contains a wildcard
                if (path.Contains("*"))
                {

                }

                // if testUrl starts with the forbidden one
                if (testUrl.Length >= path.Length && testUrl.StartsWith(path))
                {
                    return true;
                }

            }

            return false;
        }

        private IEnumerable<string> ParseRobotsFile(string fileText, string userAgent)
        {
            if (fileText == null)
            {
                yield break;
            }

            // split into lines (removing empties)
            var splitResult = fileText.ToLower().Split(
                new string[] { Environment.NewLine },
                StringSplitOptions.RemoveEmptyEntries);

            var listenToThisLine = false;

            // find the user agent lines
            foreach (var line in splitResult)
            {
                if (line.StartsWith("user-agent"))
                {
                    // turn off/on listening to disallows if this user-agent block is relevant to us
                    listenToThisLine = line.Contains("*") || line.Contains(userAgent);
                    // nothing more needed from this line, continue to next
                    continue;
                }

                if (listenToThisLine)
                {
                    // space to care about sitemaps in future

                    if (line.StartsWith("disallow"))
                    {
                        var url = line.Replace("disallow: ", "");

                        if (!url.Contains("disallow") && !url.Contains(" ") && url.Length == 0)
                        {
                            yield return url;
                        }
                    }
                }
            }
        }
    }
}

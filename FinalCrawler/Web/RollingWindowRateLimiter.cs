using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinalCrawler.Abstractions;
using FinalCrawler.Abstractions.Web;

namespace FinalCrawler.Web
{
    public class RollingWindowRateLimiter : IRateLimiter
    {
        private readonly INowProvider _nowProvider;
        private readonly long _window;
        private readonly int _maxRequestsInWindow;
        private readonly Dictionary<string, List<long>> _domains
            = new Dictionary<string, List<long>>();

        public RollingWindowRateLimiter(TimeSpan windowSize, int maxRequestsInWindow, INowProvider nowProvider)
        {
            if (maxRequestsInWindow < 1)
            {
                throw new ArgumentOutOfRangeException("max requests must be at least 1");
            }

            _nowProvider = nowProvider ?? throw new ArgumentNullException(nameof(nowProvider));
            _window = windowSize.Ticks;
            _maxRequestsInWindow = maxRequestsInWindow;
        }

        public async Task HoldIfRequired(Uri uri)
        {
            // find all records for this domain
            var existingForDomain = _domains.FirstOrDefault(d => d.Key == uri.Host);

            // no key yet, add a new item for this domain and add now as a first time record
            if (string.IsNullOrEmpty(existingForDomain.Key))
            {
                _domains.TryAdd(uri.Host, new List<long> { _nowProvider.Now.Ticks });
                return;
            }

            // if none, just go to the end of the method (put in now and pass)
            if (existingForDomain.Value.Count() > 0)
            {
                // check each of them and remove any that are now stale
                var notStale = existingForDomain.Value.Where(x => _nowProvider.Now.Ticks - x < _window);

                // count how many notStale ones there are, if the count is higher than _maxRequest, we get the oldest (beginning of notStale)
                if (notStale.Count() >= _maxRequestsInWindow)
                {
                    var oldest = notStale.First();
                    // wait for the oldest stamp to be older than now - window
                    await Task.Delay(TimeSpan.FromTicks(Math.Abs(oldest - _nowProvider.Now.Ticks - _window)));
                }

                // update the collection for this domain to remove stale ones
                _domains[uri.Host] = notStale.ToList();
            }

            // add now to the collection 
            _domains[uri.Host].Add(_nowProvider.Now.Ticks);
        }
    }
}

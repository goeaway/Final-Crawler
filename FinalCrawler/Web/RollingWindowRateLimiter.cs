using System;
using System.Collections.Generic;
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
        private readonly Dictionary<string, long> _domains
            = new Dictionary<string, long>();

        public RollingWindowRateLimiter(TimeSpan windowSize, int maxRequestsInWindow, INowProvider nowProvider)
        {
            _nowProvider = nowProvider ?? throw new ArgumentNullException(nameof(nowProvider));
            _window = windowSize.Ticks;
            _maxRequestsInWindow = maxRequestsInWindow;
        }

        public async Task HoldIfRequired(Uri uri)
        {
            // add a record per request with host and time
            // if there are more than the allowed amount in the window, we wait until the oldest one goes out of the window
            // when we allow pass through we record it
        }
    }
}

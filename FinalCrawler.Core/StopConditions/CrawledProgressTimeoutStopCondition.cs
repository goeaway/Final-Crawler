using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using FinalCrawler.Core.Abstractions;

namespace FinalCrawler.Core.StopConditions
{
    public class CrawledProgressTimeoutStopCondition : ICrawlStopCondition
    {
        private int _lastCrawlCount;
        private DateTime _lastCrawledCountIncreaseDate;
        private readonly TimeSpan _waitTimeout;
        private readonly INowProvider _nowProvider;

        public CrawledProgressTimeoutStopCondition(TimeSpan waitTimeout, INowProvider nowProvider)
        {
            _waitTimeout = waitTimeout;
            _nowProvider = nowProvider ?? throw new ArgumentNullException(nameof(nowProvider));
        }

        public bool ShouldStop(CrawlReport report)
        {
            // no change, check how long it's been since the last,
            // if it's too long, return false;
            if (_lastCrawlCount == report.Crawled.Count())
            {
                return _nowProvider.Now - _waitTimeout > _lastCrawledCountIncreaseDate;
            }

            _lastCrawlCount = report.Crawled.Count();
            _lastCrawledCountIncreaseDate = _nowProvider.Now;
            return true;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using FinalCrawler.Core.Abstractions;

namespace FinalCrawler.Core.StopConditions
{
    public class MaxTimeStopCondition : ICrawlStopCondition
    {
        private readonly TimeSpan _maxTime;

        public MaxTimeStopCondition(TimeSpan maxTime)
        {
            _maxTime = maxTime;
        }

        public bool ShouldStop(CrawlReport report)
        {
            return report.TimeElapsed >= _maxTime;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using FinalCrawler.Core.Abstractions;

namespace FinalCrawler.Core.StopConditions
{
    public class MaxCrawlAmountStopCondition : ICrawlStopCondition
    {
        private readonly int _max;

        public MaxCrawlAmountStopCondition(int max)
        {
            _max = max;
        }

        public bool ShouldStop(CrawlReport report)
        {
            return report.Crawled.Count() >= _max;
        }
    }
}

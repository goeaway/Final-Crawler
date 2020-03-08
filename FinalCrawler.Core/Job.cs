using System;
using System.Collections.Generic;
using System.Text;
using FinalCrawler.Core.Abstractions;

namespace FinalCrawler.Core
{
    public class Job
    {
        public IEnumerable<Uri> Seeds { get; set; }
            = new List<Uri>();
        public bool QueueNewLinks { get; set; }
            = true;
        public IEnumerable<ICrawlStopCondition> StopConditions { get; set; }
            = new List<ICrawlStopCondition>();

        public string DataPattern { get; set; }
    }
}

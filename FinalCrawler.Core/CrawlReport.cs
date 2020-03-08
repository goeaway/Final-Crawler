using System;
using System.Collections.Generic;

namespace FinalCrawler.Core
{
    public class CrawlReport
    {
        public IEnumerable<Uri> Crawled { get; set; }
        public IEnumerable<string> Errors { get; set; }
        public int DataExtractionCount { get; set; }
        public TimeSpan TimeElapsed { get; set; }
    }
}
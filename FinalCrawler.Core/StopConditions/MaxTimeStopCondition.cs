using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using FinalCrawler.Core.Abstractions;

namespace FinalCrawler.Core.StopConditions
{
    [Serializable]
    public class MaxTimeStopCondition : ICrawlStopCondition
    {
        private readonly TimeSpan _maxTime;

        public MaxTimeStopCondition(TimeSpan maxTime)
        {
            _maxTime = maxTime;
        }

        public MaxTimeStopCondition(SerializationInfo info, StreamingContext context)
        {
            _maxTime = TimeSpan.FromTicks((long)info.GetValue("maxtime", typeof(long)));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("maxtime", _maxTime.Ticks, typeof(long));
        }

        public bool ShouldStop(CrawlReport report)
        {
            return report.TimeElapsed >= _maxTime;
        }
    }
}

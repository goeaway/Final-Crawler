using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace FinalCrawler.Core.Abstractions
{
    public interface ICrawlStopCondition : ISerializable
    {
        bool ShouldStop(CrawlReport report);
    }
}

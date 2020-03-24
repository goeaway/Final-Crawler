using System;
using System.Collections.Generic;
using System.Text;

namespace FinalCrawler.Core.Abstractions
{
    public interface INowProvider
    {
        DateTime Now { get; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace FinalCrawler.Abstractions
{
    public interface INowProvider
    {
        DateTime Now { get; }
    }
}

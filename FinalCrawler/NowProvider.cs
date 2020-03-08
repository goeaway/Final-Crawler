using System;
using System.Collections.Generic;
using System.Text;
using FinalCrawler.Abstractions;

namespace FinalCrawler
{
    public class NowProvider : INowProvider
    {
        private readonly DateTime? _now;
        public DateTime Now => _now ?? DateTime.Now;

        public NowProvider(DateTime now)
        {
            _now = now;
        }

        public NowProvider() { }
    }
}

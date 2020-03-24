using System;
using System.Collections.Generic;
using System.Text;
using FinalCrawler.Core.Abstractions;

namespace FinalCrawler.Tests.Tools
{
    public class TestNowProvider : INowProvider
    {
        private DateTime? _now;
        public DateTime Now => _now ?? DateTime.Now;

        public TestNowProvider(DateTime now)
        {
            _now = now;
        }

        public TestNowProvider()
        {

        }

        public void Update(DateTime now)
        {
            _now = now;
        }
    }
}

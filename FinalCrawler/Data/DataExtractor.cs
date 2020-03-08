using System;
using System.Collections.Generic;
using System.Text;
using FinalCrawler.Abstractions.Data;

namespace FinalCrawler.Data
{
    public class DataExtractor : IDataExtractor
    {
        public IEnumerable<string> ExtractData(string html, string pattern)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Uri> ExtractUris(string html)
        {
            throw new NotImplementedException();
        }
    }
}

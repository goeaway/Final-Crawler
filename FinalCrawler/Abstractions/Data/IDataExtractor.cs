using System;
using System.Collections.Generic;
using System.Text;

namespace FinalCrawler.Abstractions.Data
{
    public interface IDataExtractor
    {
        IEnumerable<Uri> ExtractUris(string html);
        IEnumerable<string> ExtractData(string html, string pattern);
    }
}

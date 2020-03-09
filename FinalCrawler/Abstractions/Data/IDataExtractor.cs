using System;
using System.Collections.Generic;
using System.Text;

namespace FinalCrawler.Abstractions.Data
{
    public interface IDataExtractor
    {
        void LoadCustomRegexPattern(string regexPattern);
        IEnumerable<Uri> ExtractUris(Uri source, string html);
        IEnumerable<string> ExtractData(string html);
    }
}

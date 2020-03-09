using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using FinalCrawler.Abstractions.Data;

namespace FinalCrawler.Data
{
    public class DataExtractor : IDataExtractor
    {
        private const string URI_REGEX_PATTERN = "<a.*?href=\"(.*?)\"";
        private readonly Regex _uriRegex = new Regex(URI_REGEX_PATTERN);
        private Regex _customRegex;

        public void LoadCustomRegexPattern(string regexPattern)
        {
            _customRegex = new Regex(regexPattern ?? "");
        }

        public IEnumerable<string> ExtractData(string html)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Uri> ExtractUris(Uri source, string html)
        {
            var matches = _uriRegex.Matches(html);

            foreach (Match match in matches)
            {
                if (match.Success)
                {
                    var value = match.Groups[1].Value;

                    if (!value.StartsWith("https://") && !value.StartsWith("http://") && !value.StartsWith(source.Host))
                    {
                        value = "http://" + source.Host + (!value.StartsWith("/") ? "/" : "") + value;

                        if (!value.EndsWith("/"))
                        {
                            value += "/";
                        }
                    }

                    yield return new Uri(value);
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using CommandLine;

namespace FinalCrawler.CLI
{
    public class Options
    {
        [Option('t', "threads", Default = 1, HelpText = "Amount of threads to use when crawling, min: 1, max: 64")]
        public int Threads { get; set; }
    }
}

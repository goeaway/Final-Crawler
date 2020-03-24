using System;
using System.Collections.Generic;
using System.Text;
using CommandLine;

namespace FinalCrawler.CLI
{
    public class Options
    {
        [Option('t', "threads", Default = 1, HelpText = "Amount of threads to use when crawling, min: 1, max: 64")]
        public uint Threads { get; set; }

        [Option('p', "path", Required = true, HelpText = "Path to a json file defining a job to be run or a directory where job files will be placed. Files must have the .fc.json extension and will be deleted once the job is complete.")]
        public string Path { get; set; }
    }
}

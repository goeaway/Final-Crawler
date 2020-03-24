using FinalCrawler.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FinalCrawler.CLI.Abstractions
{
    internal interface IJobLoader
    {
        Task<(string, Job)> LoadJob(string path);
        Task DeleteJob(string path);
    }
}

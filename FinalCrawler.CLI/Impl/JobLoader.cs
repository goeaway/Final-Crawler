using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinalCrawler.CLI.Abstractions;
using FinalCrawler.Core;
using Newtonsoft.Json;

namespace FinalCrawler.CLI.Impl
{
    internal class JobLoader : IJobLoader
    {
        public Task DeleteJob(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            return Task.CompletedTask;
        }

        public async Task<(string, Job)> LoadJob(string path)
        {
            var pathToReadFrom = path;

            // if it's a directory get the first file in the directory with .fc.json
            if (File.GetAttributes(path).HasFlag(FileAttributes.Directory))
            {
                pathToReadFrom = Directory.GetFiles(path).Where(f => f.EndsWith(".fc.json")).FirstOrDefault();
            }

            return (
                path, 
                pathToReadFrom != null ? 
                    JsonConvert.DeserializeObject<Job>(await File.ReadAllTextAsync(pathToReadFrom)) : 
                    null);
        }
    }
}

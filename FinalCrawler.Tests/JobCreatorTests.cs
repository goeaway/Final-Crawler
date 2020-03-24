using System;
using System.Collections.Generic;
using System.IO;
using FinalCrawler.Core;
using FinalCrawler.Core.Abstractions;
using FinalCrawler.Core.StopConditions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace FinalCrawler.Tests
{
    [TestClass]
    [TestCategory("Tools")]
    public class JobCreatorTests
    {
        private const string SAVE_PATH = @"C:\crawler create jobs here";

        [TestMethod]
        public void CreateAndSaveJobFile()
        {
            var job = new Job
            {
                Seeds = new List<Uri>
                {
                    new Uri("http://localhost"),
                    new Uri("https://www.google.com")
                },
                DataPattern = "<img.*?src=\"(.*?)\"",
                QueueNewLinks = true,
            };

            using (var file = File.Create(Path.Combine(SAVE_PATH, Guid.NewGuid().ToString() + ".fc.json")))
            using (var writer = new StreamWriter(file))
            {
                writer.Write(JsonConvert.SerializeObject(job));
            }
        }
    }
}

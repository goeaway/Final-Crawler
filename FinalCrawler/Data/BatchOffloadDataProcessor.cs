using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FinalCrawler.Abstractions.Data;

namespace FinalCrawler.Data
{
    public class BatchOffloadDataProcessor : IDataProcessor
    {
        public Task ProcessData(Uri source, IEnumerable<string> data)
        {
            throw new NotImplementedException();
        }
    }
}

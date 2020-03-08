using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FinalCrawler.Abstractions.Data
{
    public interface IDataProcessor
    {
        Task ProcessData(Uri source, IEnumerable<string> data);
    }
}

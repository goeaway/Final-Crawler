using System;
using System.Collections.Generic;
using System.Text;

namespace FinalCrawler.Abstractions.Data
{
    public interface IBloomFilter
    {
        void Add(string item);
        bool Contains(string item);
    }
}

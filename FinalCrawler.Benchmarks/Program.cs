using System;
using BenchmarkDotNet.Running;

namespace FinalCrawler.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<BloomFilterBenchmarks>();
        }
    }
}

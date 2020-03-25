using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BenchmarkDotNet.Attributes;
using FinalCrawler.Data;

namespace FinalCrawler.Benchmarks
{
    [MemoryDiagnoser]
    public class BloomFilterBenchmarks
    {
        private const string EXISTS_TEST_STRING = "testing exists";
        private const string ADD_TEST_STRING = "testing add";

        private RichardKundlBloomFilter _bloomFilter;
        private ConcurrentBag<string> _bag;

        [Params(1000, 100_000, 1_000_000, 10_000_000)]
        public int N;

        [GlobalSetup]
        public void Setup()
        {
            _bloomFilter = new RichardKundlBloomFilter(N);
            _bag = new ConcurrentBag<string>();

            foreach (var i in Enumerable.Range(0, N))
            {
                _bag.Add(i + " test string " + i);
            }
        }

        [Benchmark]
        public bool BloomFilterContains() => _bloomFilter.Contains(EXISTS_TEST_STRING);

        [Benchmark]
        public bool BagContains() => _bag.Contains(EXISTS_TEST_STRING);
    }
}

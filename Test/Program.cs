using System;
using System.IO;
using System.Diagnostics;
using LinearTsvParser;
using BenchmarkDotNet.Running;

namespace TsvTests {
    public class Program
    {
        [Conditional("BENCHMARK")]
        private static void Benchmark() {
            BenchmarkRunner.Run<TsvBenchmarkTest>();
        }

        public static void Main(string[] args)
        {
            Program.Benchmark();
        }
    }
}

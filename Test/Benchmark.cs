using System;
using System.IO;
using LinearTsvParser;
using System.Linq;
using System.Collections.Generic;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Attributes;

namespace TsvTests {
    [MemoryDiagnoser]
    public class TsvBenchmarkTest {
        private const int BatchSize = 120000;

        [Benchmark]
        public void LibReadTest() {
            using (var mem = new MemoryStream())
            using (var writer = new StreamWriter(mem))
            using (var tsv = new TsvReader(mem)) {
                Random rnd = new Random();

                for (int i = 0; i < BatchSize; i++) {
                    writer.Write($"{rnd.Next(1, 10000)}\t{rnd.Next(1, 10000)}\t{rnd.Next(1, 10000)}\t{rnd.Next(1, 10000)}" +
                        $"\t{rnd.Next(1, 10000)}\t{rnd.Next(1, 10000)}\t{rnd.Next(1, 10000)}\t{rnd.Next(1, 10000)}\n");
                }

                writer.Flush();
                mem.Flush();
                mem.Position = 0;

                for (int i = 0; i < BatchSize; i++) {
                    var result = tsv.ReadLine();
                }
            }
        }

        [Benchmark]
        public void NativeReadTest() {
            using (var mem = new MemoryStream())
            using (var writer = new StreamWriter(mem))
            using (var reader = new StreamReader(mem)) {
                Random rnd = new Random();

                for (int i = 0; i < BatchSize; i++) {
                    writer.Write($"{rnd.Next(1, 10000)}\t{rnd.Next(1, 10000)}\t{rnd.Next(1, 10000)}\t{rnd.Next(1, 10000)}" +
                        $"\t{rnd.Next(1, 10000)}\t{rnd.Next(1, 10000)}\t{rnd.Next(1, 10000)}\t{rnd.Next(1, 10000)}\n");
                }

                writer.Flush();
                mem.Flush();
                mem.Position = 0;

                for (int i = 0; i < BatchSize; i++) {
                    var line = reader.ReadLine();
                    
                    var result = line.Split('\t').Select(x => x.Replace("\\\\", "\0").Replace("\\r", "\r").Replace("\\n", "\n")
                        .Replace("\\t", "\t").Replace("\0", "\\")).ToList();
                }
            }
        }

        [Benchmark]
        public void LibWriteTest() {
            using (var mem = new MemoryStream())
            using (var reader = new StreamReader(mem))
            using (var tsv = new TsvWriter(mem)) {
                Random rnd = new Random();
                List<string> fields = new List<string>();

                for (int i = 0; i < BatchSize; i++) {
                    fields.Clear();
                    for (int j = 0; j < 10; j++) {
                        fields.Add(rnd.Next(1, 10000).ToString());
                    }

                    tsv.WriteLine(fields);
                }

                tsv.Flush();
                mem.Flush();
                mem.Position = 0;

                for (int i = 0; i < BatchSize; i++) {
                    var result = reader.ReadLine();
                }
            }
        }

        [Benchmark]
        public void NativeWriteTest() {
            using (var mem = new MemoryStream())
            using (var reader = new StreamReader(mem))
            using (var writer = new StreamWriter(mem)) {
                Random rnd = new Random();
                List<string> fields = new List<string>();

                for (int i = 0; i < BatchSize; i++) {
                    fields.Clear();
                    for (int j = 0; j < 10; j++) {
                        fields.Add(rnd.Next(1, 10000).ToString().Replace("\\", "\\\\").Replace("\t", "\\t")
                            .Replace("\r", "\\r").Replace("\n", "\\n"));
                    }

                    writer.WriteLine(String.Join('\t', fields));
                }

                writer.Flush();
                mem.Flush();
                mem.Position = 0;

                for (int i = 0; i < BatchSize; i++) {
                    var result = reader.ReadLine();
                }
            }
        }
    }
}

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
        [Benchmark]
        public void LibReadTest() {
            List<List<string>> tmp = new List<List<string>>();

            using (var mem = new MemoryStream())
            using (var writer = new StreamWriter(mem))
            using (var tsv = new TsvReader(mem)) {
                Random rnd = new Random();

                for (int i = 0; i < 60000; i++) {
                    writer.Write($"{rnd.Next(1, 10000)}\t{rnd.Next(1, 10000)}\t{rnd.Next(1, 10000)}\t{rnd.Next(1, 10000)}" +
                        $"\t{rnd.Next(1, 10000)}\t{rnd.Next(1, 10000)}\t{rnd.Next(1, 10000)}\t{rnd.Next(1, 10000)}\n");
                    writer.Flush();
                    mem.Position = 0;
                    
                    tmp.Add(tsv.ReadLine());
                }
            }
        }

        [Benchmark]
        public void NativeReadTest() {
            List<List<string>> tmp = new List<List<string>>();

            using (var mem = new MemoryStream())
            using (var writer = new StreamWriter(mem))
            using (var reader = new StreamReader(mem)) {
                Random rnd = new Random();

                for (int i = 0; i < 60000; i++) {
                    writer.Write($"{rnd.Next(1, 10000)}\t{rnd.Next(1, 10000)}\t{rnd.Next(1, 10000)}\t{rnd.Next(1, 10000)}" +
                        $"\t{rnd.Next(1, 10000)}\t{rnd.Next(1, 10000)}\t{rnd.Next(1, 10000)}\t{rnd.Next(1, 10000)}\n");
                    writer.Flush();
                    mem.Position = 0;

                    var line = reader.ReadLine();
                    
                    tmp.Add(line.Split('\t').Select(x => x.Replace("\\\\", "\0").Replace("\\r", "\r").Replace("\\n", "\n")
                        .Replace("\\t", "\t").Replace("\0", "\\")).ToList());
                }
            }
        }

        [Benchmark]
        public void LibWriteTest() {
            List<string> tmp = new List<string>();

            using (var mem = new MemoryStream())
            using (var reader = new StreamReader(mem))
            using (var tsv = new TsvWriter(mem)) {
                Random rnd = new Random();
                List<string> fields = new List<string>();

                for (int i = 0; i < 60000; i++) {
                    fields.Clear();
                    for (int j = 0; j < 10; j++) {
                        fields.Add(rnd.Next(1, 10000).ToString());
                    }

                    tsv.WriteLine(fields);
                    tsv.Flush();
                    mem.Position = 0;

                    tmp.Add(reader.ReadLine());
                }
            }
        }

        [Benchmark]
        public void NativeWriteTest() {
            List<string> tmp = new List<string>();

            using (var mem = new MemoryStream())
            using (var reader = new StreamReader(mem))
            using (var writer = new StreamWriter(mem)) {
                Random rnd = new Random();
                List<string> fields = new List<string>();

                for (int i = 0; i < 60000; i++) {
                    fields.Clear();
                    for (int j = 0; j < 10; j++) {
                        fields.Add(rnd.Next(1, 10000).ToString().Replace("\\", "\\\\").Replace("\t", "\\t")
                            .Replace("\r", "\\r").Replace("\n", "\\n"));
                    }

                    writer.WriteLine(String.Join('\t', fields));
                    mem.Position = 0;

                    tmp.Add(reader.ReadLine());
                }
            }
        }
    }
}

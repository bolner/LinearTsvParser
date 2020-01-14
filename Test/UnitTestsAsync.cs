using System;
using System.IO;
using System.Threading.Tasks;
using LinearTsvParser;
using Xunit;

namespace TsvTests {
    public class UnitTestsAsync {
        [Fact]
        public static async Task TestRead() {
            using (var mem = new MemoryStream())
            using (var writer = new StreamWriter(mem))
            using (var tsv = new TsvReader(mem)) {
                for (int i = 0; i < 10; i++) {
                    await writer.WriteAsync("a\\r\tb\\n\tc\\t\td\\\\\\n\n");
                }

                writer.Flush();
                mem.Position = 0;

                for (int i = 0; i < 10; i++) {
                    var line = await tsv.ReadLineAsync();

                    Assert.Equal("a\r", line[0]);
                    Assert.Equal("b\n", line[1]);
                    Assert.Equal("c\t", line[2]);
                    Assert.Equal("d\\\n", line[3]);
                }
            }
        }

        [Fact]
        public static async Task TestWrite() {
            using (var mem = new MemoryStream())
            using (var reader = new StreamReader(mem))
            using (var tsv = new TsvWriter(mem)) {
                for (int i = 0; i < 10; i++) {
                    await tsv.WriteLineAsync(
                        new string[] {"a\r", "b\n", "c\t", "d\\\n"}
                    );
                }

                tsv.Flush();
                mem.Position = 0;

                for (int i = 0; i < 10; i++) {
                    string line = await reader.ReadLineAsync();

                    Assert.Equal("a\\r\tb\\n\tc\\t\td\\\\\\n", line);
                }
            }
        }
    }
}

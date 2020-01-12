Linear TSV Parser
=================

Reading and writing `Linear TSV` files in a safe, lossless way.

# NuGet package

Available at: https://www.nuget.org/packages/LinearTsvParser

To include it in a `.NET Core` project:

    dotnet add package LinearTsvParser

# Examples

Reading a `.tsv.gz` file:

```csharp
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using LinearTsvParser;

public class Example {
    public void ReadTsv() {
        using (var input = File.OpenRead("/tmp/test.tsv.gz"))
        using (var gzip = new GZipStream(input, CompressionMode.Decompress))
        using (var tsvReader = new TsvReader(gzip)) {
            while(!tsvReader.EndOfStream) {
                List<string> fields = tsvReader.ReadLine();
            }
        }
    }
}
```

Writing a `.tsv.gz` file:

```csharp
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using LinearTsvParser;

public class Example {
    public void WriteTsv(List<string[]> data) {
        using (var outfile = File.Create("/tmp/test.tsv.gz"))
        using (var gzip = new GZipStream(outfile, CompressionMode.Compress))
        using (var tsvWriter = new TsvWriter(gzip)) {
            tsvWriter.WriteLine(new List<string>{ "One", "Two\tTwo", "Three" });

            foreach(string[] fields in data) {
                tsvWriter.WriteLine(fields);
            }
        }
    }
}
```

The writer accepts any `enumerable` of strings, let it be `string[]` or `List<string>`.

You can output the TSV to the standard output by using `Console.Out` in the constructor:

```csharp
using System;
using System.IO;
using LinearTsvParser;

public class Example {
    public void WriteTsv() {
        using var tsvWriter = new TsvWriter(Console.Out));

        tsvWriter.WriteLine(new string[] {"One", "Two", "Three"});
    }
}
```

# The `Linear TSV` format

- Fields are separated by TAB characters
- Text encoding is `UTF-8`
- The reader can parse lines with any of these three endings: `\n`, `\r\n`, `\r`
- The writer is restricted to output only the `\n` character as EOL
- Special characters inside the fields are replaced (both ways):
  - Newline => `"\n"`
  - Carriage return => `"\r"`
  - Tab => `"\t"`
  - `"\"` (backslash) => `"\\"`
- The column counts are not validated, they can vary per line.

# Benchmark

The benchmark test compares the performace of this library with "native" solutions, which use string replace operations. The solution with string replace (native) uses slightly more memory and is a bit slover than this library (lib). The benchmark test can be found here: [BenchTest.cs](Test/Benchmark.cs)

|          Method |     Mean |    Error |   StdDev | Allocated |
|---------------- |---------:|---------:|---------:|----------:|
|     LibReadTest | 275.5 ms |  7.15 ms | 21.08 ms |  62.31 MB |
|  NativeReadTest | 309.8 ms | 10.08 ms | 29.26 ms |  66.66 MB |
|    LibWriteTest | 110.8 ms |  2.81 ms |  8.25 ms |  23.52 MB |
| NativeWriteTest | 195.9 ms |  4.16 ms | 11.99 ms |  36.06 MB |

# Configurations

Run the unit tests: (You can also run them one-by-one from VS Code)
```bash
dotnet build -c Debug
dotnet test
```

Run the benchmark:
```bash
dotnet run -c Release
```

Create package for NuGet:
```bash
dotnet build -c Prod
dotnet pack -c Prod
```

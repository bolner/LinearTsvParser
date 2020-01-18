Linear TSV Parser
=================

Reading and writing `Linear TSV` files in a safe, lossless way. Both async and sync I/O operations are supported.

# NuGet package

Available at: https://www.nuget.org/packages/LinearTsvParser

To include it in a `.NET Core` project:

```bash
$ dotnet add package LinearTsvParser
```

# Examples

Reading a `.tsv.gz` file in async mode:

```csharp
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Collections.Generic;
using LinearTsvParser;

public class Example {
    public async Task ReadTsv() {
        using var input = File.OpenRead("/tmp/test.tsv.gz");
        using var gzip = new GZipStream(input, CompressionMode.Decompress);
        using var tsvReader = new TsvReader(gzip);

        while(!tsvReader.EndOfStream) {
            List<string> fields = await tsvReader.ReadLineAsync();
        }
    }
}
```

Writing a `.tsv.gz` file in sync mode:

```csharp
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using LinearTsvParser;

public class Example {
    public void WriteTsv(List<string[]> data) {
        using var outfile = File.Create("/tmp/test.tsv.gz");
        using var gzip = new GZipStream(outfile, CompressionMode.Compress);
        using var tsvWriter = new TsvWriter(gzip);

        tsvWriter.WriteLine(new List<string>{ "One", "Two\tTwo", "Three" });

        foreach(string[] fields in data) {
            tsvWriter.WriteLine(fields);
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

You can ensure that the data hits the physical drive by calling `FlushAsync`.
```csharp
public async Task WriteTsv(Stream output) {
    using var tsvWriter = new TsvWriter(output);
    
    await tsvWriter.WriteLineAsync(new List<string>{ "One", "Two\tTwo", "Three" });
    await tsvWriter.FlushAsync();
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

The benchmark test compares the performace of this library with "native" solutions, which use string replace operations. The solution with string replace (native) uses more memory and is slower than this library (lib). The benchmark test can be found here: [Benchmark.cs](Test/Benchmark.cs)

|          Method |     Mean |   Error |  StdDev | Allocated |
|---------------- |---------:|--------:|--------:|----------:|
|     LibReadTest | 225.0 ms | 4.42 ms | 5.90 ms | 139.51 MB |
|  NativeReadTest | 309.7 ms | 3.84 ms | 3.59 ms | 148.66 MB |
|    LibWriteTest | 158.4 ms | 1.92 ms | 1.71 ms |   72.4 MB |
| NativeWriteTest | 270.6 ms | 5.36 ms | 6.38 ms |  86.11 MB |

# Configurations

Run the unit tests: (You can also run them one-by-one from VS Code)
```bash
$ dotnet build -c Debug
$ dotnet test
```

Run the benchmark:
```bash
$ dotnet run -c Release
```

Create package for NuGet:
```bash
$ dotnet build -c Prod
$ dotnet pack -c Prod
```

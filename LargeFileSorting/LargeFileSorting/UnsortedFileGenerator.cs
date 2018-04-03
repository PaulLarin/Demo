using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LargeFileSorting
{
    partial class Program
    {
        static class UnsortedFileGenerator
        {
            public static String Generate(string fileName, int n)
            {
                using (var sw = new StreamWriter(fileName))
                {
                    for (int i = 0; i < n; i++)
                    {
                        sw.WriteLine(GetRandomEntry().ConvertToString());
                    }

                    sw.Flush();
                }

                Console.WriteLine($"Generated {fileName}"); 

                return fileName;
            }

            public static void Generate(string fileName, long size)
            {
                var sw = Stopwatch.StartNew();

                using (var fs = File.Create(fileName, 4048, FileOptions.Asynchronous))
                using (var bs = new BufferedStream(fs))
                using (var swr = new StreamWriter(bs))
                {
                    var bytesWritten = 0L;
                    do
                    {
                        var entry = GetRandomEntry().ConvertToString();
                        var entrySize = System.Text.Encoding.UTF8.GetByteCount(entry);

                        swr.WriteLine(entry);

                        bytesWritten += entrySize;
                    } while (bytesWritten <= size);

                    swr.Flush();
                }
                sw.Stop();
                Console.WriteLine($"Generated {fileName}, generation time: {sw.Elapsed}");
            }

            static Random Random = new Random();

            static Entry GetRandomEntry()
            {
                var number = Random.Next(101);
                var value = PhrasesFactory.GetRandomPhrase();
                var entry = new Entry(number, value);

                return entry;
            }

            public class PhrasesFactory
            {
                public static List<string> Phrases { get; } = new List<string>
            {
                "Banana is yellow",
                "Apple",
                "Cherry is the best",
                "Strawbery is sweet",
                "Stars is light",
                "Orange is orange",
                "Lemon is yellow",
                "Lime is Green",
                "Mango is like a pin",
                "Something something something",
            };

                static Random Random = new Random();

                public static string GetRandomPhrase()
                {
                    var ind = Random.Next(0, Phrases.Count - 1);

                    return Phrases[ind];
                }
            }
        }

    }
}

using Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace UnsortedFileGenerator
{
    public static partial class UnsortedFileGenerator
    {
        public static String Generate(string fileName, int n)
        {
            Console.WriteLine($"generating unsorted file..");

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

        static Random Random = new Random();

        private static Entry GetRandomEntry()
        {
            var number = Random.Next(100);
            var value = PhrasesFactory.GetRandomPhrase();
            var entry = new Entry(number, value);

            return entry;
        }

        public static void Generate(string fileName, long size)
        {
            Console.WriteLine($"Generating unsorted file..");

            var sw = Stopwatch.StartNew();

            var progressStep = size / 10;
            var progressPosition = 0L;
            using (var tw = File.Create(fileName))
            using (var bw = new BufferedStream(tw))
            using (var swr = new StreamWriter(bw))
            {
                var bytesWritten = 0L;
                do
                {
                    if (bytesWritten > progressPosition)
                    {
                        progressPosition += progressStep;
                        Console.Write($"{100 * bytesWritten / size}%..");
                    }

                    var entry = GetRandomEntry().ConvertToString();
                    var entrySize = System.Text.Encoding.UTF8.GetByteCount(entry + "\n\r");
                    swr.WriteLine(entry);
                    bytesWritten += entrySize;

                } while (bytesWritten <= size);

                swr.Flush();
            }
            sw.Stop();

            Console.Write($"100%..");
            Console.WriteLine();
            Console.WriteLine($"Generated {fileName}, generation time: {sw.Elapsed}");
        }
    }
}



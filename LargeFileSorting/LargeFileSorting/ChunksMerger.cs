using System;
using System.IO;
using System.Linq;

namespace LargeFileSorting
{
    static class ChunksMerger
    {
        public static void MergeIntoSortedFile(SplitResult splitResult, string outFilePath)
        {
            var chunkReaders = splitResult.Chunks.Select(x =>
            {
                var sr = File.OpenRead(x.Path);
                var bs = new BufferedStream(sr, 10 * 1024 * 1024);
                return new StreamReader(bs);
            }).ToDictionary(x => x, x => new Entry(x.ReadLine()));

            var progressStep = splitResult.TotalEntriesCount / 10;
            var totalEntriesCount = splitResult.TotalEntriesCount;

            var min = chunkReaders.FirstOrDefault(y => y.Value == chunkReaders.Min(x => x.Value));

            int i = 0;
            using (var sw = File.CreateText(outFilePath))
            {
                while (chunkReaders.Any())
                {
                    sw.WriteLine(min.Value.ConvertToString());

                    var next = min.Key.ReadLine();
                    if (next == null)
                    {
                        chunkReaders.Remove(min.Key);
                        min = chunkReaders.FirstOrDefault(x => x.Value == chunkReaders.Min(y => y.Value));
                        continue;
                    }
                    var entry = new Entry(next);
                    chunkReaders[min.Key] = entry;

                    if (i++ % progressStep == 0)
                        Console.Write($"{(int)(100 * (((double)i) / totalEntriesCount))}% ");

                    if (entry.CompareTo(min.Value) == 1)
                        min = chunkReaders.FirstOrDefault(x => x.Value == chunkReaders.Min(y => y.Value));
                }

                Console.Write("100%");
                Console.WriteLine();

                sw.Close();
            }
        }
    }
}
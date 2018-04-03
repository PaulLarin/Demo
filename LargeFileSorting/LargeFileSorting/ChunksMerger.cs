using System;
using System.IO;
using System.Linq;

namespace LargeFileSorting
{
    static class ChunksMerger
    {
        public static void MergeIntoSortedFile(SplitResult sortResult, string outFilePath)
        {
            int chunksCount = sortResult.Chunks.Count();
            var totalEntriesCount = sortResult.TotalEntriesCount;
            var chunks = sortResult.Chunks.ToArray();
            var firstEntries = new Entry[chunksCount];
            var tempFilesReaders = new StreamReader[chunksCount];

            for (int i = 0; i < chunksCount; i++)
            {
                var fs = new FileStream(chunks[i].Path, FileMode.Open);
                var bs = new BufferedStream(fs);

                tempFilesReaders[i] = new StreamReader(bs);

                var first = tempFilesReaders[i].ReadLine();
                if (first != null)
                    firstEntries[i] = new Entry(first);
            }

            var progressStep = totalEntriesCount / 10;

            using (var sw = File.CreateText(outFilePath))
            {
                for (int i = 0; i < totalEntriesCount; i++)
                {
                    var minEntry = firstEntries[0];
                    var minEntryChunk = 0;

                    for (int j = 0; j < chunksCount; j++)
                    {
                        var entry = firstEntries[j];
                        if (entry == null)
                            continue;

                        if (minEntry == null)
                        {
                            minEntry = firstEntries[j];
                            minEntryChunk = j;
                        }

                        if ((minEntry.CompareTo(entry) >= 0))
                        {
                            minEntry = firstEntries[j];
                            minEntryChunk = j;
                        }
                    }

                    if (minEntry != null)
                        sw.WriteLine(minEntry.ConvertToString());

                    var line = tempFilesReaders[minEntryChunk].ReadLine();
                    if (line != null)
                        firstEntries[minEntryChunk] = new Entry(line);
                    else
                        firstEntries[minEntryChunk] = null;

                    if (i % progressStep == 0)
                        Console.Write($"{(int) (100 * (((double) i) / totalEntriesCount))}% ");
                }
            }

            Console.Write($"100% ");


            foreach (var sr in tempFilesReaders)
                sr.Close();

            Console.WriteLine();
            Console.WriteLine($"Sub blocks merged into {outFilePath}");
            Console.WriteLine();
        }



    }
}
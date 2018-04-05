using Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LargeFileSorting
{
    static class ChunksSorter
    {
        public static void SortParallel(SplitResult splitResult, long maxAllowedMemorySize)
        {
            var parallelProcessingList = new List<List<ChunkInfo>>() { new List<ChunkInfo>() };

            var commonParallelProcessingSize = 0L;

            foreach (var chunk in splitResult.Chunks)
            {
                commonParallelProcessingSize += new FileInfo(chunk.Path).Length;
                if (commonParallelProcessingSize < maxAllowedMemorySize)
                    parallelProcessingList.Last().Add(chunk);
                else
                {
                    parallelProcessingList.Add(new List<ChunkInfo>() { chunk });
                    commonParallelProcessingSize = new FileInfo(chunk.Path).Length;
                }
            }

            foreach (var parallelProcessinChunks in parallelProcessingList)
            {
                Parallel.ForEach(parallelProcessinChunks, SortChunk);

                GC.Collect();
            }
        }

        static void SortChunk(ChunkInfo chunk)
        {
            var entries = new Entry[chunk.ItemsCount];

            using (var fs = new FileStream(chunk.Path, FileMode.Open, FileAccess.Read))
            using (var sr = new StreamReader(fs))
            {
                for (int i = 0; i < chunk.ItemsCount; i++)
                {
                    var line = sr.ReadLine();
                    var entry = new Entry(line);

                    entries[i] = entry;
                }

                fs.Close();
                sr.Close();
            }

            var sw = Stopwatch.StartNew();

            // т.к. известно, что есть повторяющиеся части String, вместо сортировки entries сделаем упорядоченную группировку по String:
            var groupedByString = entries.GroupBy(x => x.String).OrderBy(x => x.Key);

            using (var tw = File.OpenWrite(chunk.Path))
            using (var swr = new StreamWriter(tw))
            {
                foreach (var group in groupedByString)
                {
                    // далее каждую группу отсортируем по Number и запишем в чанк:
                    var ordered = group.OrderBy(x => x.Number);
                    foreach (var entry in ordered)
                        swr.WriteLine(entry.ConvertToString());
                }

                swr.Flush();
                swr.Close();
                tw.Close();
            }

            Console.WriteLine($"chunk sorted, time: {sw.Elapsed}, items count: {chunk.ItemsCount.ToString("N0")}");
        }
    }

}
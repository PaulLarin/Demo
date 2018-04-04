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
        public static void Sort(SplitResult splitResult, long maxAllowedMemorySize)
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
            using (var bs = new BufferedStream(fs))
            using (var sr = new StreamReader(bs))
            {
                for (int i = 0; i < chunk.ItemsCount; i++)
                {
                    var line = sr.ReadLine();
                    entries[i] = new Entry(line);
                }

                fs.Close();
                bs.Close();
                sr.Close();
            }

            var sw = Stopwatch.StartNew();

            Array.Sort(entries);

            Console.WriteLine($"chunk sorted, time: {sw.Elapsed}, items count: {chunk.ItemsCount.ToString("N0")}");

            using (var tw = File.OpenWrite(chunk.Path))
            using (var swr = new StreamWriter(tw))
            {
                foreach (var entry in entries)
                    swr.WriteLine(entry.ConvertToString());

                swr.Flush();
                swr.Close();
                tw.Close();
            }
        }
    }

}
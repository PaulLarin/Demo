using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Data;

namespace LargeFileSorting
{
    public static class FileSorter
    {
        public static void Sort(string filePath, string outFilePath)
        {
            try
            {
                var chunksSortingConsumingCollection = new BlockingCollection<ChunkInfo>();

                var parallelChunksSortingDeegre = 4;

                var chunkSortingTasks = StartSortingConsuming(chunksSortingConsumingCollection, parallelChunksSortingDeegre);

                var fileSize = new FileInfo(filePath).Length;

                Console.WriteLine($"file size: {fileSize.ToMb()} Mb");

                var sw = Stopwatch.StartNew();

                var maxAllowedRamSize = 4000L * 1024 * 1024;

                var maxChunkSize = (fileSize > maxAllowedRamSize ? maxAllowedRamSize : fileSize) / parallelChunksSortingDeegre;

                var options = new SplitOptions(maxChunkSize);

                var splitResult = FileSplitter.SplitIntoChunks(filePath, options, chunksSortingConsumingCollection.Add);

                var splitTime = sw.Elapsed;

                chunksSortingConsumingCollection.CompleteAdding();

                Task.WaitAll(chunkSortingTasks);

                Console.WriteLine();
                Console.WriteLine($"chunks count: {splitResult.Chunks.Count()}");
                Console.WriteLine($"entries count: {splitResult.TotalEntriesCount.ToString("N0")}");

                Console.WriteLine($"chunks sort time: {sw.Elapsed}");
                Console.WriteLine($"file split time: {splitTime}");
                sw.Restart();

                ChunksMerger.MergeIntoSortedFile(splitResult, outFilePath);
                Console.WriteLine($"merge time: {sw.Elapsed}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static Task[] StartSortingConsuming(BlockingCollection<ChunkInfo> collection, int parallelDeegre)
        {            
            var consumerTasks = new Task[parallelDeegre];

            for (int i = 0; i < parallelDeegre; i++)
            {
                consumerTasks[i] = Task.Factory.StartNew(() =>
                {
                    foreach (var chunk in collection.GetConsumingEnumerable())
                        ChunksSorter.SortChunk(chunk);

                    GC.Collect();
                });
            }

            return consumerTasks;
        }
    }
}
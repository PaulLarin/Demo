using System;
using System.Diagnostics;
using System.IO;
using Common;

namespace LargeFileSorting
{
    public static class FileSorter
    {
        public static void Sort(string filePath, string outFilePath)
        {
            try
            {
                var fileSize = new FileInfo(filePath).Length;
                Console.WriteLine($"file size: {fileSize.ToMb()} Mb");

                var sw = Stopwatch.StartNew();

                var maxAllowedRamSize = 4000L * 1024 * 1024;

                var maxChunkSize = (fileSize > maxAllowedRamSize ? maxAllowedRamSize : fileSize) / 4;

                var options = new SplitOptions(maxChunkSize);

                var splitResult = FileSplitter.SplitIntoChunks(filePath, options);

                Console.WriteLine($"file split time: {sw.Elapsed}");
                sw.Restart();

                ChunksSorter.SortParallel(splitResult, maxAllowedRamSize);

                Console.WriteLine($"chunks sort time: {sw.Elapsed}");
                sw.Restart();

                ChunksMerger.MergeIntoSortedFile(splitResult, outFilePath);
                Console.WriteLine($"merge time: {sw.Elapsed}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }


    }
}
using System;
using System.Diagnostics;
using System.IO;

namespace LargeFileSorting
{
    class FileSorter
    {
        string _filePath;
        string _outFilePath;

        public FileSorter(string filePath, string outFilePath)
        {
            _filePath = filePath;
            _outFilePath = outFilePath;
        }

        public void Sort()
        {
            var fileSize = new FileInfo(_filePath).Length;
            Console.WriteLine($"file size: {fileSize.ToMb()} Mb");

            var sw = Stopwatch.StartNew();

            var maxAllowedRamSize = 1000L * 1024 * 1024;

            var maxChunkSize = fileSize > maxAllowedRamSize ? maxAllowedRamSize / 10 : fileSize / 10;

            var options = new SplitOptions(maxChunkSize);

            var splitResult = FileSplitter.SplitIntoChunks(_filePath, options);

            Console.WriteLine($"file split time: {sw.Elapsed}");
            sw.Restart();

            ChunksSorter.Sort(splitResult, maxAllowedRamSize);

            Console.WriteLine($"chunks sort time: {sw.Elapsed}");
            sw.Restart();

            ChunksMerger.MergeIntoSortedFile(splitResult, _outFilePath);
            Console.WriteLine($"merge time: {sw.Elapsed}");
        }


    }
}
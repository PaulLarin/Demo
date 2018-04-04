using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LargeFileSorting
{
    public class SplitOptions
    {
        public SplitOptions(long maxChunkSize)
        {
            MaxChunkSize = maxChunkSize;
        }

        public long MaxChunkSize { get; }
    }

    public static class FileSplitter
    {
        private static string GetChunkFilePath(string inputFile, int number)
        {
            return $"TEMP_{Path.GetFileNameWithoutExtension(inputFile)}" + "\\temp" + number.ToString() + ".txt";
        }

        public static SplitResult SplitIntoChunks(string filePath, SplitOptions options)
        {
            CreateTempDir(filePath);

            var fileSize = new FileInfo(filePath).Length;
            var maxChunkSize = options.MaxChunkSize;
            var chunksCount = 0;
            var totalEntriesCount = 0;

            var minEntriesPerChunk = 1000000;

            Console.WriteLine();
            Console.WriteLine($"generating chunks..");

            var chunkInfos = new List<ChunkInfo>();
            var buffer = new List<Entry>((int)minEntriesPerChunk);

            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (var bs = new BufferedStream(fs))
            using (var sr = new StreamReader(bs))
            {
                string line = string.Empty;
                while (line != null)
                {
                    int i = 0;

                    var chunkFile = GetChunkFilePath(filePath, chunksCount);
                    using (var tw = File.OpenWrite(chunkFile))
                    using (var sw = new StreamWriter(tw))
                    {
                        var bytesWritten = 0;
                        while (bytesWritten < maxChunkSize)
                        {
                            line = sr.ReadLine();
                            if (line != null)
                            {
                                sw.WriteLine(line);
                                i++;
                                var lineSize = System.Text.Encoding.UTF8.GetByteCount(line);

                                bytesWritten += lineSize;
                            }
                            else
                                break;
                        }

                        sw.Flush();
                        sw.Close();
                        tw.Close();
                    }

                    Console.WriteLine($"chunk {chunksCount} generated..");

                    chunkInfos.Add(new ChunkInfo(chunkFile, i));
                    totalEntriesCount += i;

                    chunksCount++;
                }
            }

            Console.WriteLine();
            Console.WriteLine($"chunks count: {chunksCount}");
            Console.WriteLine($"entries count: {totalEntriesCount.ToString("N0")}");
            Console.WriteLine();

            return new SplitResult(chunkInfos, totalEntriesCount);
        }

        private static void CreateTempDir(string filePath)
        {
            var tempdDir = $"TEMP_{Path.GetFileNameWithoutExtension(filePath)}";

            if (Directory.Exists(tempdDir))
                Directory.Delete(tempdDir, true);

            Directory.CreateDirectory(tempdDir);
        }
    }
}
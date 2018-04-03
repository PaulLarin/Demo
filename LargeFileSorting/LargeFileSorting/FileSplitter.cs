using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LargeFileSorting
{
    public static class FileSplitter
    {
        private static string GetChunkFilePath(string inputFile, int number)
        {
            return $"TEMP_{Path.GetFileNameWithoutExtension(inputFile)}" + "\\temp" + number.ToString() + ".txt";
        }

        public static SplitResult SplitIntoSortedChunks(string filePath)
        {
            var tempdDir = $"TEMP_{Path.GetFileNameWithoutExtension(filePath)}";

            if (Directory.Exists(tempdDir))
                Directory.Delete(tempdDir, true);

            Directory.CreateDirectory(tempdDir);

            var fileSize = new FileInfo(filePath).Length;
            var maxAllowedMemorySize = 4L * 1024 * 1024 * 1024;
            var maxEntrySize = sizeof(int) + 1024 * sizeof(char);
            var maxEntriesPerChunk = maxAllowedMemorySize / maxEntrySize / 4;

            var chunksCount = 0;
            var totalEntriesCount = 0;

            Console.WriteLine($"file size: {fileSize / 1024 / 1024} Mb");
            Console.WriteLine($"max allowed memory size: {maxAllowedMemorySize / 1024 / 1024} Mb");
            Console.WriteLine($"max entries per chunk: {maxEntriesPerChunk.ToString("N0")}");
            Console.WriteLine($"max entry size: {maxEntrySize} b");
            Console.WriteLine();
            Console.WriteLine($"generating chunks..");


            var chunkInfos = new List<ChunkInfo>();

            var buffer = new Entry[maxEntriesPerChunk];

            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (var bs = new BufferedStream(fs))
            using (var sr = new StreamReader(bs))
            {
                string line = string.Empty;
                while (line != null)
                {
                    for (int i = 0; i < maxEntriesPerChunk; i++)
                    {
                        line = sr.ReadLine();
                        if (line != null)
                        {
                            buffer[i] = new Entry(line);
                            totalEntriesCount++;
                        }
                        else
                            buffer[i] = null;
                    }

                    GC.Collect();

                    Array.Sort(buffer);

                    var chunkFile = GetChunkFilePath(filePath, chunksCount);

                    var entriesCount = 0;
                    using (var tw = File.OpenWrite(chunkFile))
                    using (var sw = new StreamWriter(tw))
                    {
                        for (int i = 0; i < buffer.Length; i++)
                        {
                            var entry = buffer[i];
                            if (entry != null)
                            {
                                var str = entry.ConvertToString();
                                sw.WriteLine(str);
                                entriesCount++;
                            }
                        }
                        sw.Flush();
                        sw.Close();
                        tw.Close();
                    }

                    Console.WriteLine($"chunk {chunksCount} generated..");

                    chunkInfos.Add(new ChunkInfo(chunkFile, entriesCount));

                    chunksCount++;
                }
            }


            Console.WriteLine();
            Console.WriteLine($"chunks count: {chunksCount}");
            Console.WriteLine($"entries count: {totalEntriesCount.ToString("N0")}");
            Console.WriteLine();

            Console.WriteLine($"Sub blocks sorted and saved in temp files");

            return new SplitResult(chunkInfos, totalEntriesCount);
        }


        public static SplitResult SplitIntoChunks(string filePath)
        {
            var tempdDir = $"TEMP_{Path.GetFileNameWithoutExtension(filePath)}";

            if (Directory.Exists(tempdDir))
                Directory.Delete(tempdDir, true);

            Directory.CreateDirectory(tempdDir);

            var fileSize = new FileInfo(filePath).Length;
            var maxAllowedMemorySize = 4L * 1024 * 1024 * 1024;
            var maxEntrySize = sizeof(int) + 1024 * sizeof(char);
            var maxEntriesPerChunk = maxAllowedMemorySize / maxEntrySize / 4;

            var chunksCount = 0;
            var totalEntriesCount = 0;

            Console.WriteLine($"file size: {fileSize / 1024 / 1024} Mb");
            Console.WriteLine($"max allowed memory size: {maxAllowedMemorySize / 1024 / 1024} Mb");
            Console.WriteLine($"max entries per chunk: {maxEntriesPerChunk.ToString("N0")}");
            Console.WriteLine($"max entry size: {maxEntrySize} b");
            Console.WriteLine();
            Console.WriteLine($"generating chunks..");

            var chunkInfos = new List<ChunkInfo>();

            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (var bs = new BufferedStream(fs))
            using (var sr = new StreamReader(bs))
            {
                string line = string.Empty;
                while (line != null)
                {
                    int i;

                    var chunkFile = GetChunkFilePath(filePath, chunksCount);
                    using (var tw = File.OpenWrite(chunkFile))
                    using (var sw = new StreamWriter(tw))
                    {

                        for ( i = 0; i < maxEntriesPerChunk; i++)
                        {
                            line = sr.ReadLine();
                            if (line != null)
                            {
                                sw.WriteLine(line);
                                totalEntriesCount++;
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

                    chunksCount++;
                }
            }

            Console.WriteLine();
            Console.WriteLine($"chunks count: {chunksCount}");
            Console.WriteLine($"entries count: {totalEntriesCount.ToString("N0")}");
            Console.WriteLine();

            Console.WriteLine($"Sub blocks sorted and saved in temp files");

            return new SplitResult(chunkInfos, totalEntriesCount);
        }


        public static SplitResult SplitIntoSortedChunks2(string filePath)
        {
            var tempdDir = $"TEMP_{Path.GetFileNameWithoutExtension(filePath)}";

            if (Directory.Exists(tempdDir))
                Directory.Delete(tempdDir, true);

            Directory.CreateDirectory(tempdDir);

            var fileSize = new FileInfo(filePath).Length;
            var maxAllowedMemorySize = 4L * 1024 * 1024 * 1024;
            var maxEntrySize = sizeof(int) + 1024 * sizeof(char);
            var maxEntriesPerChunk = maxAllowedMemorySize / maxEntrySize / 4;

            var chunksCount = 0;
            var totalEntriesCount = 0;

            Console.WriteLine($"file size: {fileSize / 1024 / 1024} Mb");
            Console.WriteLine($"max allowed memory size: {maxAllowedMemorySize / 1024 / 1024} Mb");
            Console.WriteLine($"max entries per chunk: {maxEntriesPerChunk.ToString("N0")}");
            Console.WriteLine($"max entry size: {maxEntrySize} b");
            Console.WriteLine();
            Console.WriteLine($"generating chunks..");


            var chunkInfos = new List<ChunkInfo>();


            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (var bs = new BufferedStream(fs))
            using (var sr = new StreamReader(bs))
            {
                var buffer = new string[maxEntriesPerChunk];
                string line = string.Empty;
                while (line != null)
                {

                    for (int i = 0; i < maxEntriesPerChunk; i++)
                    {
                        line = sr.ReadLine();
                        if (line != null)
                        {
                            buffer[i] = line;
                            totalEntriesCount++;
                        }
                        else
                            break;
                    }

                    buffer = buffer.Where(x => x != null).ToArray();

                    Array.Sort(buffer, new EntryStringComparer());

                    var chunkFile = GetChunkFilePath(filePath, chunksCount);

                    var entriesCount = 0;
                    using (var tw = File.OpenWrite(chunkFile))
                    using (var sw = new StreamWriter(tw))
                    {
                        for (int i = 0; i < buffer.Length; i++)
                        {
                            var entry = buffer[i];
                            if (entry != null)
                            {
                                var str = entry;
                                sw.WriteLine(str);
                            }
                        }

                        entriesCount++;

                        sw.Flush();
                        sw.Close();
                        tw.Close();
                    }

                    Console.WriteLine($"chunk {chunksCount} generated..");

                    chunkInfos.Add(new ChunkInfo(chunkFile, entriesCount));

                    chunksCount++;
                }
            }


            Console.WriteLine();
            Console.WriteLine($"chunks count: {chunksCount}");
            Console.WriteLine($"entries count: {totalEntriesCount.ToString("N0")}");
            Console.WriteLine();

            Console.WriteLine($"Sub blocks sorted and saved in temp files");

            return new SplitResult(chunkInfos, totalEntriesCount);
        }
    }
}
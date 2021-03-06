﻿using Common;
using Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public static SplitResult SplitIntoChunks(string filePath, SplitOptions options, Action<ChunkInfo> chunkGeneratedCallback)
        {
            CreateTempDir(filePath);
            var fileSize = new FileInfo(filePath).Length;
            var maxChunkSize = options.MaxChunkSize;

            var minEntriesPerChunk = 100000;

            Console.WriteLine();
            Console.WriteLine($"generating chunks..");

            var chunksCount = 0;
            var totalEntriesCount = 0;

            var chunkInfos = new List<ChunkInfo>();
            var buffer = new List<Entry>((int)minEntriesPerChunk);

            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (var bs = new BufferedStream(fs))
            using (var sr = new StreamReader(bs))
            {
                string line = string.Empty;
                while (line != null)
                {
                    var swt = Stopwatch.StartNew();

                    int i = 0;

                    var chunkFile = GetChunkFilePath(filePath, chunksCount);

                    using (var tw = File.OpenWrite(chunkFile))
                    using (var bw = new BufferedStream(tw))
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
                    }

                    Console.WriteLine($"chunk {chunksCount} generated, size: {new FileInfo(chunkFile).Length.ToMb()}Mb, time: {swt.Elapsed}");

                    var chunk = new ChunkInfo(chunkFile, i);

                    chunkInfos.Add(chunk);

                    chunkGeneratedCallback?.Invoke(chunk);

                    totalEntriesCount += i;
                    chunksCount++;
                }
            }

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
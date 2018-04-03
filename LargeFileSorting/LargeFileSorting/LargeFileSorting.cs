using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LargeFileSorting
{
    partial class Program
    {
        class LargeFileSorter
        {
            string _filePath;
            string _outFilePath;

            private string _tempDir;
            private int _totalEntriesCount;
            private int _chunksCount;

            public LargeFileSorter(string filePath, string outFilePath)
            {
                _filePath = filePath;
                _outFilePath = outFilePath;

                var fileName = Path.GetFileNameWithoutExtension(_filePath);
                _tempDir = $"TEMP_{fileName}";
            }

            public void Sort()
            {
                if (Directory.Exists(_tempDir))
                    Directory.Delete(_tempDir, true);

                Directory.CreateDirectory(_tempDir);

                GenerateSortedChunks();
                MergeChunksIntoOutFile();

                Directory.Delete(_tempDir, true);
            }

            private void GenerateSortedChunks()
            {
                var fileSize = new FileInfo(_filePath).Length;
                var maxAllowedMemorySize = 4L * 1024 * 1024 * 1024;
                var maxEntrySize = sizeof(int) + 1024 * sizeof(char);
                var maxEntriesPerChunk = maxAllowedMemorySize / maxEntrySize / 4;

                var chunksCount = 0;
                var totalEntriesCount = 0;

                Contract.Ensures(_totalEntriesCount == totalEntriesCount);
                Contract.Ensures(_chunksCount == chunksCount);

                Console.WriteLine($"file size: {fileSize / 1024 / 1024} Mb");
                Console.WriteLine($"max allowed memory size: {maxAllowedMemorySize / 1024 / 1024} Mb");
                Console.WriteLine($"max entries per chunk: {maxEntriesPerChunk.ToString("N0")}");
                Console.WriteLine($"max entry size: {maxEntrySize} b");
                Console.WriteLine();
                Console.WriteLine($"generating chunks..");

                var col = new BlockingCollection<(Entry[], int)>();
                var col2 = new BlockingCollection<(Entry[], int)>();


                var canc = new CancellationTokenSource();

                var tasks = new List<Task>();

                for (int i = 0; i < 4; i++)
                {
                    tasks.Add(Task.Factory.StartNew(() =>
                    {
                        foreach (var t in col.GetConsumingEnumerable())
                        {
                            Array.Sort(t.Item1);


                            col2.TryAdd(t);


                        }

                    }));
                }

                var ttt = (Task.Factory.StartNew(() =>
                {
                    foreach (var t in col2.GetConsumingEnumerable(canc.Token))
                    {
                        using (var tw = File.OpenWrite(GetTempFileName(t.Item2)))
                        using (var sw = new StreamWriter(tw))
                        {
                            for (int i = 0; i < maxEntriesPerChunk; i++)
                            {
                                var entry = t.Item1[i];
                                if (entry != null)
                                {
                                    var str = entry.ConvertToString();
                                    sw.WriteLine(str);
                                }
                            }
                            sw.Flush();
                            sw.Close();
                            tw.Close();
                        }

                        Console.WriteLine($"chunk {t.Item2} generated..");
                    }
                }));

                using (var fs = new FileStream(_filePath, FileMode.Open, FileAccess.Read))
                using (var bs = new BufferedStream(fs))
                using (var sr = new StreamReader(bs))
                {

                    string line = string.Empty;
                    while (line != null)
                    {
                        var buffer = new Entry[maxEntriesPerChunk];
                        var lastNotNullIndex = maxEntriesPerChunk;

                        for (int i = 0; i < maxEntriesPerChunk; i++)
                        {
                            line = sr.ReadLine();
                            if (line != null)
                            {
                                buffer[i] = new Entry(line);
                                totalEntriesCount++;
                            }
                            else
                            {
                                lastNotNullIndex = i;
                                break;
                            }
                        }

                        col.Add((buffer, chunksCount));

                        chunksCount++;
                    }
                }

                col.CompleteAdding();
                Task.WaitAll(tasks.ToArray());

                col2.CompleteAdding();

                ttt.Wait();


                Console.WriteLine();
                Console.WriteLine($"chunks count: {chunksCount}");
                Console.WriteLine($"entries count: {totalEntriesCount.ToString("N0")}");
                Console.WriteLine();

                _chunksCount = chunksCount;
                _totalEntriesCount = totalEntriesCount;

                Console.WriteLine($"Sub blocks sorted and saved in temp files");
            }

            private void MergeChunksIntoOutFile()
            {
                var firstEntries = new Entry[_chunksCount];
                var tempFilesReaders = new StreamReader[_chunksCount];

                for (int i = 0; i < _chunksCount; i++)
                {
                    var fs = new FileStream(GetTempFileName(i), FileMode.Open);
                    var bs = new BufferedStream(fs);

                    tempFilesReaders[i] = new StreamReader(bs);

                    var first = tempFilesReaders[i].ReadLine();
                    if (first != null)
                        firstEntries[i] = new Entry(first);
                }

                var progressStep = _totalEntriesCount / 10;

                using (var fw = File.CreateText(_outFilePath))
                {
                    for (int i = 0; i < _totalEntriesCount; i++)
                    {
                        var minEntry = firstEntries[0];
                        var minEntryChunk = 0;

                        for (int j = 0; j < _chunksCount; j++)
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

                        fw.WriteLine(minEntry);

                        var line = tempFilesReaders[minEntryChunk].ReadLine();
                        if (line != null)
                            firstEntries[minEntryChunk] = new Entry(line);
                        else
                            firstEntries[minEntryChunk] = null;

                        if (i % progressStep == 0)
                            Console.Write($"{(int)(100 * (((double)i) / _totalEntriesCount))}% ");
                    }
                }

                Console.Write($"100% ");


                foreach (var sr in tempFilesReaders)
                    sr.Close();

                Console.WriteLine();
                Console.WriteLine($"Sub blocks merged into {_outFilePath}");
                Console.WriteLine();
            }

            private string GetTempFileName(int number)
            {
                return _tempDir + "\\temp" + number.ToString() + ".txt";
            }
        }

    }
}

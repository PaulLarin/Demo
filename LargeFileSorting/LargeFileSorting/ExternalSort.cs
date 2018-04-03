using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;

namespace LargeFileSorting
{
    partial class Program
    {
        public class Algorithm1
        {
            public static int TotalElements { get; private set; }

            public static void Sort(string workingDir, String fileName, string outFile)
            {
                if(Directory.Exists(workingDir + "\\TEMP"))
                    Directory.Delete(workingDir + "\\TEMP" , true);
                Directory.CreateDirectory(workingDir + "\\TEMP");

                try
                {
                    //int blocksCount = (int)Math.Ceiling((double)TotalElements / ElementsPerBlock);
                    int blocksCount = 0;


                    blocksCount = BuildSortedBlocks(workingDir, fileName, blocksCount);

                    MergeBlocks(workingDir, blocksCount, outFile);

                    Verificate(fileName, outFile);
                }
                catch (FileNotFoundException e)
                {
                    //e.printStackTrace();
                }
                catch (IOException e)
                {
                    //e.printStackTrace();
                }
            }

            private static int BuildSortedBlocks(string workingDir, string fileName, int blocksCount)
            {

                var fileSize = new FileInfo(fileName).Length;
                var maxMemorySize = 25600;
                var entrySize = sizeof(int) + 32 * sizeof(char);
                var maxEntriesPerBlock = maxMemorySize / entrySize;

                blocksCount = 0;
                var totalEntriesCount = 0;

                Console.WriteLine(maxEntriesPerBlock);

                using (var fs = new FileStream(fileName, FileMode.Open))
                using (var bs = new BufferedStream(fs))
                using (var sr = new StreamReader(bs))
                {
                    var buffer = new Entry[maxEntriesPerBlock];


                    string line = "";
                    while (line != null)
                    {
                        for (int j = 0; j < maxEntriesPerBlock; j++)
                        {
                            line = sr.ReadLine();
                            if(line!=null)
                            {
                                buffer[j] =  new Entry(line);
                                totalEntriesCount++;
                            }
                            else
                                buffer[j] = null;
                        }


                        var sorted = buffer.OrderBy(x => x);

                        if (buffer.FirstOrDefault() == null)
                            break;

                        using (var tw = File.OpenWrite(GetTempFileName(workingDir, blocksCount)))
                        using (var sw = new StreamWriter(tw))
                        {
                            foreach (var entry in sorted)
                            {
                                if (entry != null)
                                {
                                    var str = entry.ConvertToString();
                                    sw.WriteLine(str);

                                }

                                //Console.WriteLine(entry);
                            }
                        }

                        blocksCount++;
                    }
                }

                TotalElements = totalEntriesCount;

                Console.WriteLine($"Sub blocks sorted and saved in temp files");

                return blocksCount;
            }


            private static string MergeBlocks(string workingDir, int blocksCount, string outFile)
            {

                //Now open each file and merge them, then write back to disk
                var firstEntries = new Entry[blocksCount];
                var tempFilesReaders = new StreamReader[blocksCount];

                for (int i = 0; i < blocksCount; i++)
                {
                    var fs = new FileStream(GetTempFileName(workingDir, i), FileMode.Open);
                    var bs = new BufferedStream(fs);

                    tempFilesReaders[i] = new StreamReader(bs);

                    var first = tempFilesReaders[i].ReadLine();
                    if (first != null)
                        firstEntries[i] = new Entry(first);
                }

                using (var fw = File.CreateText(outFile))
                {
                    for (int i = 0; i < TotalElements; i++)
                    {
                        var minEntry = firstEntries[0];
                        var minFile = 0;

                        for (int j = 0; j < blocksCount; j++)
                        {
                            var entry = firstEntries[j];
                            if (entry == null)
                                continue;

                            if (minEntry == null)
                            {
                                minEntry = firstEntries[j];
                                minFile = j;
                            }

                            if ((minEntry.CompareTo(entry) >= 0 ))
                            {
                                minEntry = firstEntries[j];
                                minFile = j;
                            }

                        }

                        fw.WriteLine(minEntry);

                        var line = tempFilesReaders[minFile].ReadLine();
                        if (line != null)
                            firstEntries[minFile] = new Entry(line);
                        else
                        {
                            firstEntries[minFile] = null;
                        }
                    }

                    //for (int i = 0; i < blocksCount; i++)
                    //{
                    //    tempFilesReaders[i].Close();
                    //    File.Delete(GetTempFileName(workingDir, i));
                    //}
                }

                Console.WriteLine($"Sub blocks merged into {outFile}");
                Console.WriteLine();

                return outFile;
            }


            private static string GetTempFileName(string workingDir, int number)
            {
                return workingDir +"\\" + "TEMP" + "\\temp" + number.ToString() + ".txt";
            }



        }

    }
}

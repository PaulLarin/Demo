using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;

namespace LargeFileSorting
{
    partial class Program
    {
        //public class Algorithm2
        //{
        //    static string EntryStringFormat = "{0}. {1}";
        //    internal class EntryStringComparer : IComparer<string>
        //    {
        //        public int Compare(string x, string y)
        //        {
        //            var splited = x.Split(new[] { ". " }, StringSplitOptions.RemoveEmptyEntries);
        //            var splited2 = y.Split(new[] { ". " }, StringSplitOptions.RemoveEmptyEntries);

        //            if (splited.Count() != 2)
        //                throw new ArgumentException($"wrong {nameof(x)} format, {nameof(x)} = {x}");

        //            if (splited.Count() != 2)
        //                throw new ArgumentException($"wrong {nameof(x)} format, {nameof(x)} = {x}");

        //            var res = splited[1].CompareTo(splited2[1]);
        //            if (res != 0)
        //                return res;

        //            if (!int.TryParse(splited[0], out var number))
        //                throw new ArgumentException($"wrong 'number' value: {splited[0]}, {nameof(x)} = {x}");

        //            if (!int.TryParse(splited2[0], out var number2))
        //                throw new ArgumentException($"wrong 'number' value: {splited2[0]}, {nameof(y)} = {y}");

        //            return number.CompareTo(number2);
        //        }
        //    }

        //    private static IComparer<string> _comparer = new EntryStringComparer();

        //    public static void Sort(string workingDir, String fileName, string outFile)
        //    {
        //        try
        //        {
        //            int blocksCount = (int)Math.Ceiling((double)N / M);

        //            SortBlocks(workingDir, fileName, blocksCount);

        //            MergeBlocks(workingDir, blocksCount, outFile);

        //            Verificate(fileName, outFile);
        //        }
        //        catch (FileNotFoundException e)
        //        {
        //            //e.printStackTrace();
        //        }
        //        catch (IOException e)
        //        {
        //            //e.printStackTrace();
        //        }
        //    }

        //    private static void SortBlocks(string workingDir, string fileName, int blocksCount)
        //    {
        //        var buffer = new string[M < N ? M : N];

        //        using (var fs = new FileStream(fileName, FileMode.Open))
        //        using (var bs = new BufferedStream(fs))
        //        using (var sr = new StreamReader(bs))
        //        {
        //            // Iterate through the elements in the file
        //            for (int i = 0; i < blocksCount; i++)
        //            {
        //                // Read M-element chunk at a time from the file
        //                for (int j = 0; j < (M < N ? M : N); j++)
        //                {
        //                    var line = sr.ReadLine();
        //                    if (line == null)
        //                        break;

        //                    buffer[j] = line;
        //                }

        //                // Sort M elements
        //                var sorted = buffer.OrderBy(x => x, _comparer);

        //                // Write the sorted numbers to temp file

        //                using (var tw = File.OpenWrite(GetTempFileName(workingDir, i)))
        //                using (var sw = new StreamWriter(tw))
        //                {
        //                    foreach (var entry in sorted)
        //                    {
        //                        sw.WriteLine(entry);
        //                        //Console.WriteLine(entry);
        //                    }
        //                }
        //                //Console.WriteLine();
        //                //File.WriteAllLines(GetTempFileName(i), sorted);
        //            }
        //        }

        //        Console.WriteLine($"Sub blocks sorted and saved in temp files");
        //    }


        //    private static string MergeBlocks(string workingDir, int blocksCount, string outFile)
        //    {
        //        //Now open each file and merge them, then write back to disk
        //        var firstEntries = new string[blocksCount];
        //        var tempFilesReaders = new StreamReader[blocksCount];

        //        for (int i = 0; i < blocksCount; i++)
        //        {
        //            var fs = new FileStream(GetTempFileName(workingDir, i), FileMode.Open);
        //            var bs = new BufferedStream(fs);

        //            tempFilesReaders[i] = new StreamReader(bs);

        //            var first = tempFilesReaders[i].ReadLine();
        //            if (first != null)
        //                firstEntries[i] = first;
        //        }

        //        using (var fw = File.CreateText(outFile))
        //        {
        //            for (int i = 0; i < N; i++)
        //            {
        //                var minEntry = firstEntries[0];
        //                var minFile = 0;

        //                for (int j = 0; j < blocksCount; j++)
        //                {
        //                    if (_comparer.Compare(minEntry, firstEntries[j]) >= 0)
        //                    {
        //                        minEntry = firstEntries[j];
        //                        minFile = j;
        //                    }
        //                }

        //                fw.WriteLine(minEntry);

        //                var line = tempFilesReaders[minFile].ReadLine();
        //                if (line != null)
        //                    firstEntries[minFile] = line;
        //            }

        //            for (int i = 0; i < blocksCount; i++)
        //            {
        //                tempFilesReaders[i].Close();
        //                File.Delete(GetTempFileName(workingDir, i));
        //            }
        //        }

        //        Console.WriteLine($"Sub blocks merged into {outFile}");
        //        Console.WriteLine();

        //        return outFile;
        //    }

        //    private static void Verificate(string fileName, string outFile)
        //    {
        //        return;

        //        var strs1 = File.ReadLines(fileName).ToArray();
        //        var strs2 = File.ReadLines(outFile).ToArray();

        //        Contract.Assert(strs1.Count() == strs2.Count());

        //        for (int i = 0; i < strs1.Count(); i++)
        //        {
        //            Console.WriteLine($"{strs1[i]}\t\t{strs2[i]}");
        //        }

        //        foreach (var s in strs1)
        //        {
        //            var c1 = strs1.Count(x => x == s);
        //            var c2 = strs2.Count(x => x.TrimEnd() == s);

        //            Contract.Assert(c1 == c2);
        //        }

        //        for (int i = 1; i < strs2.Count(); i++)
        //        {
        //            Contract.Assert(_comparer.Compare(strs2[i], strs2[i - 1]) >= 0);
        //        }
        //    }

        //    private static string GetTempFileName(string workingDir, int number)
        //    {
        //        return workingDir + "\\temp" + number.ToString() + ".txt";
        //    }



        //}

    }
}

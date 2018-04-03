using Sortiously;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LargeFileSorting
{
    partial class Program
    {
        //public static int N = 200000; // size of the file in disk
        //public static int M = 20000; // max items the memory buffer can hold

        static void Main(string[] args)
        {
            var workingDir = "D:\\Test\\";

            Directory.CreateDirectory(workingDir);

            var inputFile = workingDir + "unsorted.txt";
            var outfile = workingDir + "sorted.txt";

            var fileSize = 500L * 1024 * 1024;

            var file = new FileInfo(inputFile);

            if (!file.Exists || file.Length != fileSize)
                UnsortedFileGenerator.Generate(inputFile, fileSize);

            var sw = Stopwatch.StartNew();

            var sorter = new LargeFileSorter(inputFile, outfile);
            sorter.Sort2();

            sw.Stop();

            Console.WriteLine(sw.Elapsed);

            Verificate(inputFile, outfile);

            Console.Read();
        }


        private static void Verificate(string fileName, string outFile)
        {
            //return;

            var strs1 = File.ReadLines(fileName).ToArray();
            var strs2 = File.ReadLines(outFile).ToArray();

            Contract.Assert(strs1.Count() == strs2.Count());

            //for (int i = 0; i < strs1.Count(); i++)
            //    Console.WriteLine($"{strs1[i]}\t\t{strs2[i]}");

            var h1 = new HashSet<string>(strs1);
            var h2 = new HashSet<string>(strs2);

            var counts1 = new Dictionary<string, int>();
            var counts2 = new Dictionary<string, int>();
            foreach (var s in strs1)
            {
                if (!counts1.ContainsKey(s))
                    counts1.Add(s, strs1.Count(x => x == s));
            }

            foreach (var s in strs2)
            {
                if (!counts2.ContainsKey(s))
                    counts2.Add(s, strs2.Count(x => x == s));
            }

            Contract.Assert(counts1.Count() == counts2.Count());


            foreach (var entry in counts1)
                Contract.Assert(counts2.ContainsKey(entry.Key) && counts2[entry.Key] == entry.Value);

            var emptyCount = strs1.Count(x => x == "");
            var emptyCount2 = strs2.Count(x => x == "");

            for (int i = 1; i < strs2.Count(); i++)
                Contract.Assert(new Entry(strs2[i]).CompareTo(new Entry(strs2[i - 1])) >= 0);

            Console.WriteLine("ok");
        }
    }
}

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
            var N = 10000000;
            var workingDir = "D:\\Test\\";
            Directory.CreateDirectory(workingDir);

            var sw = Stopwatch.StartNew();

            var inputFile = workingDir + "unsorted.txt";
            var outfile = workingDir + "sorted.txt";
            UnsortedFileGenerator.Generate(inputFile, 10000l*1024*1024);

            Console.WriteLine(sw.Elapsed);
            sw.Restart();
            //Algorithm1.Sort(workingDir, f, workingDir + "sorted.txt");
            var sorter = new LargeFileSorter(inputFile, outfile);
            sorter.Sort();
            Console.WriteLine("alg1: " + sw.Elapsed);
            Verificate(inputFile, outfile);
            //sw.Restart();
            //Algorithm2.Sort(workingDir, f, workingDir + "sorted.txt");
            //Console.WriteLine("alg2: " + sw.Elapsed);

            sw.Stop();
            Console.Read();
        }


        private static void Verificate(string fileName, string outFile)
        {
            //return;

            var strs1 = File.ReadLines(fileName).ToArray();
            var strs2 = File.ReadLines(outFile).ToArray();

            Contract.Assert(strs1.Count() == strs2.Count());

            //for (int i = 0; i < strs1.Count(); i++)
            //{
            //    Console.WriteLine($"{strs1[i]}\t\t{strs2[i]}");
            //}

            var h1 = new HashSet<string>(strs1);
            var h2 = new HashSet<string>(strs2);

            var diff = h1.Except(h2);
            Contract.Assert(diff.Count() == 0);

            for (int i = 1; i < strs2.Count(); i++)
                Contract.Assert(new Entry(strs2[i]).CompareTo(new Entry(strs2[i - 1])) >= 0);

            Console.WriteLine("ok");
        }
    }
}

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
        static void Main(string[] args)
        {
            var workingDir = "D:\\Test\\";

            Directory.CreateDirectory(workingDir);

            var inputFile = workingDir + "unsorted.txt";
            var outfile = workingDir + "sorted.txt";

            var fileSize = 10L * 1024 * 1024;

            var file = new FileInfo(inputFile);

            if (!file.Exists || Math.Abs(file.Length - fileSize) > 10000 * 1024)
                UnsortedFileGenerator.Generate(inputFile, fileSize);

            var sw = Stopwatch.StartNew();

            var sorter = new FileSorter(inputFile, outfile);
            sorter.Sort();

            sw.Stop();

            Console.WriteLine(sw.Elapsed);

            Verificate(inputFile, outfile);

            Console.Read();
        }


        private static void Verificate(string fileName, string outFile)
        {
            Console.WriteLine("checking..");

            var fileSize = new FileInfo(fileName).Length;

            Contract.Assert(fileSize < 1000.FromMb(), $"File is large! {fileSize.ToMb()} Mb, do you realy wanna check it?");

            var strs1 = File.ReadLines(fileName).ToList();
            var strs2 = File.ReadLines(outFile).ToList();

            Contract.Assert(strs1.Count() == strs2.Count());


            var h1 = new HashSet<string>(strs1);
            var h2 = new HashSet<string>(strs2);
            Contract.Assert(h1.Count() == h2.Count());

            var diffs = h1.Except(h2);
            var diffs2 = h2.Except(h1);

            Contract.Assert(diffs.Count() == 0);
            Contract.Assert(diffs2.Count() == 0);

            for (int i = 1; i < strs2.Count(); i++)
                Contract.Assert(new Entry(strs2[i]).CompareTo(new Entry(strs2[i - 1])) >= 0);

            Console.WriteLine("file is realy sorted!");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnsortedFileGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            UnsortedFileGenerator.Generate(args[0], long.Parse(args[1]));
        }
    }
}

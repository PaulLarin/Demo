﻿using System;
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
            FileSorter.Sort(args[0], args[1]);
        }
    }
}

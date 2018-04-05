using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class Extensions
    {
        public static long ToMb(this long bytes)
        {
            return bytes / 1024 / 1024;
        }

        public static long FromMb(this int mb)
        {
            return mb * 1024L * 1024L;
        }
    }
}

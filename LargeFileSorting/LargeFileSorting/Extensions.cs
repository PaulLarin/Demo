namespace LargeFileSorting
{
    static class Extensions
    {
        public static long ToMb(this long bytes)
        {
            return bytes / 1024 / 1024;
        }

        public static long FromMb(this int mb)
        {
            return mb * 1024 * 1024;
        }
    }
}

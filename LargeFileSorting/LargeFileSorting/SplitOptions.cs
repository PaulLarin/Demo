namespace LargeFileSorting
{
    public class SplitOptions
    {
        public SplitOptions(long maxChunkSize)
        {
            MaxChunkSize = maxChunkSize;
        }

        public long MaxChunkSize { get; }
    }
}
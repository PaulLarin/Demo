using System.Collections.Generic;

namespace LargeFileSorting
{
    public class SplitResult
    {
        public int TotalEntriesCount { get; }
        public IEnumerable<ChunkInfo> Chunks { get; }

        public SplitResult(IEnumerable<ChunkInfo> chunks, int totalEntriesCount)
        {
            TotalEntriesCount = totalEntriesCount;
            Chunks = chunks;
        }
    }
}
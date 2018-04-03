namespace LargeFileSorting
{
    public class ChunkInfo
    {
        public string Path { get; set; }
        public int ItemsCount { get; }

        public ChunkInfo(string path, int itemsCount)
        {
            Path = path;
            ItemsCount = itemsCount;
        }
    }
}
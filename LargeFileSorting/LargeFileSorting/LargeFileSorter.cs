using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace LargeFileSorting
{
    class LargeFileSorter
    {
        string _filePath;
        string _outFilePath;

        private string _tempDir;
        private int _totalEntriesCount;
        private int _chunksCount;

        public LargeFileSorter(string filePath, string outFilePath)
        {
            _filePath = filePath;
            _outFilePath = outFilePath;
        }

        public void Sort()
        {
            var sw = Stopwatch.StartNew();
            var splitResult = FileSplitter.SplitIntoSortedChunks(_filePath);

            Console.WriteLine($"split time: {sw.Elapsed}");
            sw.Restart();

            //ChunksMerger.MergeIntoSortedFile(splitResult, _outFilePath);
            KwayMerge(splitResult.Chunks.Select(x => x.Path), _outFilePath);
            Console.WriteLine($"merge time: {sw.Elapsed}");
        }


        public void Sort2()
        {
            var maxAllowedMemorySize = 4L * 1024 * 1024 * 1024;

            var sw = Stopwatch.StartNew();
            var splitResult = FileSplitter.SplitIntoChunks(_filePath);
            Console.WriteLine($"split time: {sw.Elapsed}");

            var chs = new List<List<ChunkInfo>>() { new List<ChunkInfo>() };
            sw.Restart();
            var size = 0L;
            foreach (var r in splitResult.Chunks)
            {
                size += new FileInfo(r.Path).Length;
                if (size < maxAllowedMemorySize)
                    chs.Last().Add(r);
                else
                {
                    chs.Add(new List<ChunkInfo>() { r });
                    size = new FileInfo(r.Path).Length;
                }
            }

            foreach (var cl in chs)
            {
                Parallel.ForEach(cl, info =>
                {
                    var buffer = new Entry[info.ItemsCount];

                    //var lines = File.ReadAllLines(info.Path).Select(x => new Entry(x)).ToArray();
                    using (var fs = new FileStream(info.Path, FileMode.Open, FileAccess.Read))
                    using (var bs = new BufferedStream(fs))
                    using (var sr = new StreamReader(bs))
                    {
                        for (int i = 0; i < info.ItemsCount; i++)
                        {
                            var line = sr.ReadLine();
                            buffer[i] = new Entry(line);
                        }
                    }


                    Array.Sort(buffer);

                    using (var tw = File.OpenWrite(info.Path))
                    using (var swr = new StreamWriter(tw))
                    {
                        foreach (var entry in buffer)
                            swr.WriteLine(entry.ConvertToString());

                        swr.Flush();
                        swr.Close();
                        tw.Close();
                    }

                    //File.WriteAllLines(info.Path, lines.Select(x => x.ConvertToString()));
                });

                GC.Collect();
            }

            Console.WriteLine($"sort time: {sw.Elapsed}");
            sw.Restart();

            Merge(splitResult, _outFilePath);
            //ChunksMerger.MergeIntoSortedFile(splitResult, _outFilePath);
            //KwayMerge(splitResult.Chunks.Select(x => x.Path), _outFilePath);

            Console.WriteLine($"merge time: {sw.Elapsed}");
        }

        private void Merge(SplitResult splitResult, string outFilePath)
        {
            var readers = splitResult.Chunks.Select(x => new StreamReader(new BufferedStream(File.OpenRead(x.Path)))).ToDictionary(x => x, x => new Entry(x.ReadLine()));
            var progressStep = splitResult.TotalEntriesCount / 10;
            var totalEntriesCount = splitResult.TotalEntriesCount;

            var min = readers.FirstOrDefault(y => y.Value == readers.Min(x => x.Value));

            int i = 0;
            using (var sw = File.CreateText(outFilePath))
            {
                while (readers.Any())
                {
                    sw.WriteLine(min.Value.ConvertToString());

                    var next = min.Key.ReadLine();
                    if (next == null)
                    {
                        readers.Remove(min.Key);
                        min = readers.FirstOrDefault(x=>x.Value == readers.Min(y => y.Value));
                        continue;
                    }
                    var entry = new Entry(next);
                    readers[min.Key] = entry;

                    if (i++ % progressStep == 0)
                        Console.Write($"{(int)(100 * (((double)i) / totalEntriesCount))}% ");

                    if (entry.CompareTo(min.Value) == 1)
                        min = readers.FirstOrDefault(x=>x.Value == readers.Min(y => y.Value));
                }

                sw.Close();
            }

        }

        static void KwayMerge(IEnumerable<string> chunkFilePaths, string resultFilePath)
        {
            var paths = chunkFilePaths.ToArray();
            int chunks = paths.Count(); // Number of chunks
            int recordsize = 100; // estimated record size
            int records = 10000000; // estimated total # records
            int maxusage = 500000000; // max memory usage
            int buffersize = maxusage / chunks; // bytes of each queue
            double recordoverhead = 7.5; // The overhead of using Queue<>
            int bufferlen = (int)(buffersize / recordsize /
              recordoverhead); // number of records in each queue

            // Open the files
            StreamReader[] readers = new StreamReader[chunks];
            for (int i = 0; i < chunks; i++)
                readers[i] = new StreamReader(paths[i]);

            // Make the queues
            Queue<Entry>[] queues = new Queue<Entry>[chunks];
            for (int i = 0; i < chunks; i++)
                queues[i] = new Queue<Entry>(bufferlen);

            // Load the queues
            for (int i = 0; i < chunks; i++)
                LoadQueue(queues[i], readers[i], bufferlen);

            // Merge!
            StreamWriter sw = new StreamWriter(resultFilePath);
            bool done = false;
            int lowest_index, j, progress = 0;
            Entry lowest_value;
            while (!done)
            {
                // Report the progress
                if (++progress % 500000 == 0)
                    Console.Write("{0:f2}%   \r",
                      100.0 * progress / records);

                // Find the chunk with the lowest value
                lowest_index = -1;
                lowest_value = null;
                for (j = 0; j < chunks; j++)
                {
                    if (queues[j] != null)
                    {
                        if (lowest_index < 0 ||
                            queues[j].Peek().CompareTo(lowest_value) < 0)
                        {
                            lowest_index = j;
                            lowest_value = queues[j].Peek();
                        }
                    }
                }

                // Was nothing found in any queue? We must be done then.
                if (lowest_index == -1) { done = true; break; }

                // Output it
                sw.WriteLine(lowest_value);

                // Remove from queue
                queues[lowest_index].Dequeue();
                // Have we emptied the queue? Top it up
                if (queues[lowest_index].Count == 0)
                {
                    LoadQueue(queues[lowest_index],
                      readers[lowest_index], bufferlen);
                    // Was there nothing left to read?
                    if (queues[lowest_index].Count == 0)
                    {
                        queues[lowest_index] = null;
                    }
                }
            }
            sw.Close();
        }

        static void LoadQueue(Queue<Entry> queue,
            StreamReader file, int records)
        {
            for (int i = 0; i < records; i++)
            {
                if (file.Peek() < 0) break;
                queue.Enqueue(new Entry(file.ReadLine()));
            }
        }


        public class PriorityQueue<T> where T : IComparable<T>
        {
            private List<T> data;

            public PriorityQueue()
            {
                this.data = new List<T>();
            }

            public void Enqueue(T item)
            {
                data.Add(item);
                int ci = data.Count - 1; // child index; start at end
                while (ci > 0)
                {
                    int pi = (ci - 1) / 2; // parent index
                    if (data[ci].CompareTo(data[pi]) >= 0) break; // child item is larger than (or equal) parent so we're done
                    T tmp = data[ci]; data[ci] = data[pi]; data[pi] = tmp;
                    ci = pi;
                }
            }

            public T Dequeue()
            {
                // assumes pq is not empty; up to calling code
                int li = data.Count - 1; // last index (before removal)
                T frontItem = data[0];   // fetch the front
                data[0] = data[li];
                data.RemoveAt(li);

                --li; // last index (after removal)
                int pi = 0; // parent index. start at front of pq
                while (true)
                {
                    int ci = pi * 2 + 1; // left child index of parent
                    if (ci > li) break;  // no children so done
                    int rc = ci + 1;     // right child
                    if (rc <= li && data[rc].CompareTo(data[ci]) < 0) // if there is a rc (ci + 1), and it is smaller than left child, use the rc instead
                        ci = rc;
                    if (data[pi].CompareTo(data[ci]) <= 0) break; // parent is smaller than (or equal to) smallest child so done
                    T tmp = data[pi]; data[pi] = data[ci]; data[ci] = tmp; // swap parent and child
                    pi = ci;
                }
                return frontItem;
            }

            public T Peek()
            {
                T frontItem = data[0];
                return frontItem;
            }

            public int Count()
            {
                return data.Count;
            }

            public override string ToString()
            {
                string s = "";
                for (int i = 0; i < data.Count; ++i)
                    s += data[i].ToString() + " ";
                s += "count = " + data.Count;
                return s;
            }

            public bool IsConsistent()
            {
                // is the heap property true for all data?
                if (data.Count == 0) return true;
                int li = data.Count - 1; // last index
                for (int pi = 0; pi < data.Count; ++pi) // each parent index
                {
                    int lci = 2 * pi + 1; // left child index
                    int rci = 2 * pi + 2; // right child index

                    if (lci <= li && data[pi].CompareTo(data[lci]) > 0) return false; // if lc exists and it's greater than parent then bad.
                    if (rci <= li && data[pi].CompareTo(data[rci]) > 0) return false; // check the right child too.
                }
                return true; // passed all checks
            } // IsConsistent
        } // PriorityQueue
    }
}
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Index
{
    class MapReduce
    {
        public static ConcurrentBag<string> bag = new ConcurrentBag<string>();
        public BlockingCollection<string> chunks = new BlockingCollection<string>(bag);
        public ConcurrentDictionary<string, int> dict = new ConcurrentDictionary<string, int>();
        public IEnumerable<string>ProduceBlocks(string text)
        {
            int blockSize = 500;
            int startPos = 0;
            int length = 0;
            for (int i = 0; i < text.Length; i++)
            {
                i = i + blockSize > text.Length - 1 ? text.Length - 1 : i + blockSize;
                
                if (i == startPos)
                {
                    i = i + blockSize > (text.Length - 1) ? text.Length - 1 : i + blockSize;
                    length = i - startPos + 1;
                }
                else
                {
                    length = i - startPos;
                }

                yield return text.Substring(startPos, length).Trim();
                startPos = i;
            }
        }

        void Map(string text)
        {
            Parallel.ForEach(ProduceBlocks(text), wordBlock =>
            {
                string[] words = wordBlock.Split(' ');
                StringBuilder buffer = new StringBuilder();

                if (buffer.Length > 0)
                {
                    chunks.Add(buffer.ToString());
                    buffer.Clear();
                }
            });

            chunks.CompleteAdding();
        }

        void Reduce()
        {
            Parallel.ForEach(chunks.GetConsumingEnumerable(), word =>
            {
                dict.AddOrUpdate(word, 1, (key, oldValue) => Interlocked.Increment(ref oldValue));
            });
        }
        public void Run(string text)
        {
            if (chunks.IsAddingCompleted)
            {
                bag = new ConcurrentBag<string>();
                chunks = new BlockingCollection<string>(bag);
            }

            ThreadPool.QueueUserWorkItem(delegate (object state)
            {
                Map(text);
            });

            Reduce();
        }
    }
}

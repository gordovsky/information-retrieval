using Lexer;
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
        static ConcurrentBag<KeyValuePair<string, int>> bag = new ConcurrentBag<KeyValuePair<string, int>>();
        BlockingCollection<KeyValuePair<string, int>> chunks = new BlockingCollection<KeyValuePair<string, int>>(bag);
        public ConcurrentDictionary<string, SortedSet<int>> dict = new ConcurrentDictionary<string, SortedSet<int>>();
        public IEnumerable<string>ProduceBlocks(string text)
        {
            int blockSize = 500;
            int startPos = 0;
            int length = 0;
            for (int i = 0; i < text.Length; i++)
            {
                i = i + blockSize > text.Length - 1 ? text.Length - 1 : i + blockSize;
                while (i >= startPos && text[i] != ' ')
                {
                    i--;
                }

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

        void Map(Document doc)
        {
            Parallel.ForEach(ProduceBlocks(doc.Body), block =>
            {
                BasicLexer lexer = new BasicLexer();
                foreach (var lex in lexer.Tokenize(block))
                {
                    chunks.Add(new KeyValuePair<string, int>(lex, doc.Id));
                }
            });
            chunks.CompleteAdding();
        }

        void Reduce(int docId)
        {
            Parallel.ForEach(chunks.GetConsumingEnumerable(), word =>
            {
                dict.AddOrUpdate(word.Key, new SortedSet<int> { docId }, (key, value) => { value.Add(docId); return value; }); //(word, 1, (key, oldValue) => Interlocked.Increment(ref oldValue));
            });
        }
        public void Run(Document doc)
        {
            if (chunks.IsAddingCompleted)
            {
                bag = new ConcurrentBag<KeyValuePair<string,int>>();
                chunks = new BlockingCollection<KeyValuePair<string, int>>(bag);
            }

            ThreadPool.QueueUserWorkItem(delegate (object state)
            {
                Map(doc);
            });

            Reduce(doc.Id);
        }
    }
}

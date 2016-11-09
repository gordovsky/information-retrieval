using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lexer;
using System.IO;

namespace Index
{
    //class InvertedIndex
    //{
    //    public static Dictionary<TItem, IEnumerable<TKey>> Invert<TKey, TItem>(Dictionary<TKey, IEnumerable<TItem>> dictionary)
    //    {
    //        return dictionary
    //            .SelectMany(keyValuePair => keyValuePair.Value.Select(item => new KeyValuePair<TItem, TKey>(item, keyValuePair.Key)))
    //            .GroupBy(keyValuePair => keyValuePair.Key)
    //            .ToDictionary(group => group.Key, group => group.Select(keyValuePair => keyValuePair.Value));
    //    }
    //}
    class InvertedIndex
    {
        SortedDictionary<string, List<int>> index = new SortedDictionary<string, List<int>>();
        public InvertedIndex(List<Document> documents) //(Dictionary<string, string> files)
        {
            foreach (var doc in documents)
            {
                string text = doc.Body;
                BasicLexer lexer = new BasicLexer();
                foreach (var lex in lexer.Tokenize(text))
                {
                    if (!index.ContainsKey(lex))
                        index.Add(lex, new List<int>(new int[] {doc.Id}));
                    else
                    {
                        if (!index[lex].Contains(doc.Id))
                            index[lex].Add(doc.Id);
                    }
                }
            }
        }
        public KeyValuePair<string, List<int>> GetByIndex(int ind)
        {
            return index.ElementAt(ind);
        }
        public int GetCount()
        {
            return index.Count();
        }
    }
}

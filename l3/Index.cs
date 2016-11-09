using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lexer;
using System.IO;

namespace Index
{
    class InvertedIndex
    {
        public SortedList<string, int> Dictionary = new SortedList<string, int>();
        public List<List<int>> Index = new List<List<int>>();
        //public Dictionary<int, List<int>> Index = new Dictionary<int, List<int>>();
        public int Count
        {
            get { return Dictionary.Count; }
        }
        public InvertedIndex(List<Document> documents) 
        {
            foreach (var doc in documents)
            {
                string text = doc.Body;
                BasicLexer lexer = new BasicLexer();
                foreach (var lex in lexer.Tokenize(text))
                {
                    if (!Dictionary.ContainsKey(lex))
                    { 
                        Dictionary.Add(lex, Dictionary.Count);
                        Index.Add(new List<int>(doc.Id)); //Add(Dictionary.Count, new List<int>(doc.Id));
                    }
                    else
                    {
                        //var r = Dictionary.IndexOfKey(lex);
                        //var i = Index[r];
                        if (!Index[Dictionary[lex]].Contains(doc.Id))
                            Index[Dictionary.IndexOfKey(lex)].Add(doc.Id);
                    }
                }
            }
        }
        public KeyValuePair<string, int> GetByIndex(int ind)
        {
            return Dictionary.ElementAt(ind);
        }
        //public override string ToString()
        //{
        //    StringBuilder stringIndex = new StringBuilder();
        //    foreach (var term in Index)
        //    {
        //        stringIndex.Append(term.Key + ":" + string.Join(",",term.Value.ToArray()) + ";");
        //    }
        //    return stringIndex.ToString();
        //}

    }
}

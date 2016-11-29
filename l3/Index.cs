using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lexer;
using System.IO;

namespace Index
{
    interface IIndex
    {
        List<int> GetValue(string input);
    }
    class InvertedIndex : IIndex
    {
        public Dictionary<string, List<int>> Index = new Dictionary<string, List<int>>();
        public int Count
        {
            get { return Index.Count; }
        }
        public InvertedIndex(List<Document> documents) 
        {
            foreach (var doc in documents)
            {
                string text = doc.Body;
                BasicLexer lexer = new BasicLexer();
                foreach (var lex in lexer.Tokenize(text))
                {
                    if (!Index.ContainsKey(lex))
                    {
                        Index.Add(lex, new List<int>(new int[] { doc.Id}));
                    }
                    else
                    {
                        if (!Index[lex].Contains(doc.Id))
                            Index[lex].Add(doc.Id);
                    }
                }
            }
        }
        List<int> IIndex.GetValue(string input)
        {
            return Index[input];
        }
    }
}

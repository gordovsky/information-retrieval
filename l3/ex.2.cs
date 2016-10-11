using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Index
{
    public interface IRanker
    {
        IList<long> Rank(string[] terms, System.Collections.Generic.IEnumerable<long> hits);
        Dictionary<long, double> Scores { get; }
    }
    public interface ILexer
    {
        IEnumerable<string> Tokenize(string document);
    }
    public interface IParser
    {
        IEnumerable<Document> ExtractDocuments(Stream file);
    }
    public class Document
    {
        public Document(string docId, string title, string body)
        {
            this.SpecialIdentifier = docId;
            this.Body = body;
            this.Title = title;
        }

        public string SpecialIdentifier { get; private set; }
        public string Body { get; private set; }
        public string Title { get; private set; }
    }
    public class BasicLexer : ILexer
    {
        HashSet<char> ignoreList = new HashSet<char>();
        private string[] stopList;

        public BasicLexer()
        {
            char[] ignore = "\t\n.,\r-;:|()[]?! <>\"".ToCharArray();
            foreach (char c in ignore)
                this.ignoreList.Add(c);
            this.stopList =
                ("a,able,about,across,after,all,almost,also,am,among,an,and," +
                "any,are,as,at,be,because,been,but,by,can,cannot,could,dear," +
                "did,do,does,either,else,ever,every,for,from,get,got,had,has," +
                "have,he,her,hers,him,his,how,however,i,if,in,into,is,it,its," +
                "just,least,let,like,likely,may,me,might,most,must,my,neither," +
                "no,nor,not,of,off,often,on,only,or,other,our,own,rather,said," +
                "say,says,she,should,since,so,some,than,that,the,their,them,then," +
                "there,these,they,this,tis,to,too,twas,us,wants,was,we,were,what,when," +
                "where,which,while,who,whom,why,will,with,would,yet,you,your").Split(',');
        }


        private bool isValidTerm(string term)
        {
            return !this.stopList.Contains(term) &&
                !term.Contains("&") &&
                term.Length > 1;
        }
        
        public IEnumerable<string> Tokenize(string document)
        {
            StringBuilder token = new StringBuilder();
            foreach (char character in document)
            {
                if (ignoreList.Contains(character))
                {
                    if (token.Length > 0)
                    {
                        string result = token.ToString().ToLower();
                        token.Clear();
                        if (this.isValidTerm(result))    // dont return stop words
                        {
                            yield return result;
                        }
                    }
                }
                else
                {
                    token.Append(character);
                }
            }

            if (token.Length > 0)
            {
                yield return token.ToString().ToLower();
            }
        }
    }
    class InvertedIndex
    {
        public static Dictionary<TItem, IEnumerable<TKey>> Invert<TKey, TItem>(Dictionary<TKey, IEnumerable<TItem>> dictionary)
        {
            return dictionary
                .SelectMany(keyValuePair => keyValuePair.Value.Select(item => new KeyValuePair<TItem, TKey>(item, keyValuePair.Key)))
                .GroupBy(keyValuePair => keyValuePair.Key)
                .ToDictionary(group => group.Key, group => group.Select(keyValuePair => keyValuePair.Value));
        }
    }
}

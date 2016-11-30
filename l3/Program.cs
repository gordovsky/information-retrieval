using System;
using System.IO;
using Lexer;
using System.Linq;
using Index;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Compressing;
using Search;

namespace Program
{
    class Program
    {
        static void Main(string[] args)
        {
            string dataPath = @"F:\data";
            string smallDataSetPath = @"C:\Users\col403\Desktop\ir\little data set";
            string wikiDataSetPath = @"C:\Users\col403\Desktop\ir\wiki data set";
            string certainDocumentPath = @"C:\Users\col403\Desktop\ir\little data set\10.txt";
            string reutersPath = @"C:\Users\col403\Desktop\ir\reuters21578";

            Console.WriteLine("test started...");
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            var documents = GetDocuments(smallDataSetPath);
            
            MapReduce mapReduce = new MapReduce();
            foreach (var doc in documents)
            {
                mapReduce.Run(doc);
            }

            stopwatch.Stop();
            Console.WriteLine("Map reduce elapsed time: {0}", stopwatch.ElapsedMilliseconds);
            stopwatch.Reset();
            stopwatch.Start();

            InvertedIndex index = new InvertedIndex(documents);

            //var answer = BoolSearch.Search("Music AND approximately NOT composers", index);

            //Compressor.WriteStringToFile(index, @"C:\Users\col403\Desktop\ir\");
            var compDict = Compressor.CompressedDictToString(index);
            var compIndex = Compressor.CompressedInvIndexToString(index);


            stopwatch.Stop();
            Console.WriteLine("elapsed time: {0}", stopwatch.ElapsedMilliseconds);
            Console.WriteLine("fin...");
            Console.ReadKey();
        }
        public static List<Document> GetDocuments(string path)
        {
            int id = 1;
            List<Document> documents = new List<Document>();
            foreach (string doc in Directory.GetFiles(path))
            {
                string text = File.ReadAllText(doc);
                documents.Add(new Document(id, Path.GetFileName(doc), text));
                id++;
            }
            return documents;
        }
    }
}

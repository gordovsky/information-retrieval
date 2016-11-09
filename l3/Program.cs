using System;
using System.IO;
using Lexer;
using System.Linq;
using Index;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Program
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("test started...");
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();


            string dataPath = @"F:\data";
            string littleDataSetPath = @"C:\Users\col403\Desktop\ir\little data set";
            string wikiDataSetPath = @"C:\Users\col403\Desktop\ir\wiki data set";
            string certainDocumentPath = @"C:\Users\col403\Desktop\ir\little data set\10.txt";

            //string document = File.ReadAllText(certainDocumentPath);

            List<Document> documents = new List<Document>();
            int i = 1;
            foreach (string doc in Directory.GetFiles(littleDataSetPath))
            {
                string text = File.ReadAllText(doc);
                documents.Add(new Document(i, Path.GetFileName(doc), text));
                i++;
            }

            InvertedIndex index = new InvertedIndex(documents);

            var indexElement1 = index.GetByIndex(2400);
            var indexElement2 = index.GetByIndex(2900);
            //Tuple<string, List<int>> example1 = new Tuple<string, List<int>>(index[2000]);
            //BasicLexer lexer = new BasicLexer();


           
            //foreach (var w in lexer.Tokenize(document))
            //    Console.WriteLine(w);
            //string[]  = Directory.GetFiles(dataPath);

            //string[] paths = Directory.GetFiles(dataPath);



            //string[] files = Directory.GetFiles(dataPath);
            //Console.Write("find: ");
            //var find = Console.ReadLine();

            //var dictionary = files.ToDictionary(file => file, file => File.ReadAllText(file).Split().AsEnumerable());

            //var invertredDict = InvertedIndex.Invert(dictionary);
            //Console.WriteLine("{0} found in: {1}", find, string.Join(" ", InvertedIndex.Invert(dictionary)[find]));

            stopwatch.Stop();
            Console.WriteLine("elapsed time: {0}", stopwatch.ElapsedMilliseconds);
            Console.WriteLine("end...");

        }
    }
}

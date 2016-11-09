using System;
using System.IO;
using Lexer;
using System.Linq;
using Index;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Archiving;

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
            string reutersPath= @"C:\Users\col403\Desktop\ir\reuters21578";



            List<Document> documents = new List<Document>();
            int id = 1;
            foreach (string doc in Directory.GetFiles(littleDataSetPath))
            {
                string text = File.ReadAllText(doc);
                documents.Add(new Document(id, Path.GetFileName(doc), text));
                id++;
            }

            InvertedIndex index = new InvertedIndex(documents);
            
            Compressor compressor = new Compressor(index);
            compressor.WriteCompressedStringToFile(@"C:\Users\col403\Desktop\ir\");

            //index.WriteStringToFile(@"C:\Users\col403\Desktop\ir\");


            var indexElement1 = index.Dictionary.ElementAt(2500);
            var indexElement2 = index.Dictionary.ElementAt(3990);


            stopwatch.Stop();
            Console.WriteLine("elapsed time: {0}", stopwatch.ElapsedMilliseconds);
            Console.WriteLine("end...");
            Console.ReadKey();
        }
    }
}

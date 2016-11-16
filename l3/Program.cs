using System;
using System.IO;
using Lexer;
using System.Linq;
using Index;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Compressing;

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
            foreach (string doc in Directory.GetFiles(reutersPath))
            {
                string text = File.ReadAllText(doc);
                documents.Add(new Document(id, Path.GetFileName(doc), text));
                id++;
            }


            Compressor.ShiftCode(511);

            InvertedIndex index = new InvertedIndex(documents);

            Compressor.WriteStringToFile(index, @"C:\Users\col403\Desktop\ir\");
            var compDict = Compressor.CompressedDictToString(index);
            var compIndex = Compressor.CompressedInvIndexToString(index);


            using (StreamWriter writer = new StreamWriter(@"C:\Users\col403\Desktop\ir\" + "compressed_dict.txt"))
            {
                writer.Write(compDict);
            }
            using (StreamWriter writer = new StreamWriter(@"C:\Users\col403\Desktop\ir\" + "compressed_index.txt"))
            {
                writer.Write(compIndex);
            }

            //index.WriteStringToFile(@"C:\Users\col403\Desktop\ir\");


            var indexElement1 = index.Index.ElementAt(2500);
            var indexElement2 = index.Index.ElementAt(3990);


            stopwatch.Stop();
            Console.WriteLine("elapsed time: {0}", stopwatch.ElapsedMilliseconds);
            Console.WriteLine("fin...");
            Console.ReadKey();
        }
    }
}

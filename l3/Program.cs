using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Index;

namespace Program
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("test started...");

            //string dataPath = @"C:\Users\col403\Desktop\ir\wiki data set";

            string dataPath = @"F:\data";
            string[] files = Directory.GetFiles(dataPath);
            Console.Write("find: ");
            var find = Console.ReadLine();

            var dictionary = files.ToDictionary(file => file, file => File.ReadAllText(file).Split().AsEnumerable());

            var invertredDict = InvertedIndex.Invert(dictionary);
            //Console.WriteLine("{0} found in: {1}", find, string.Join(" ", InvertedIndex.Invert(dictionary)[find]));

            Console.WriteLine(invertredDict.ToString());

            Console.WriteLine("end.");

        }
    }
}

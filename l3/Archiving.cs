using System;
using System.Collections.Generic;
using Index;
using System.Text;
using System.IO;

namespace Archiving
{
    class Compressor
    {
        SortedList<string, int> Dictionary = new SortedList<string, int>();
        List<List<int>> Index = new List<List<int>>();

        public Dictionary<int, int> CompressedDictionary = new Dictionary<int, int>();
        public Compressor(InvertedIndex index)
        {
            Dictionary = index.Dictionary;
            Index = index.Index;
        }
        public void WriteStringToFile(string path)
        {
            StringBuilder s = new StringBuilder();
            foreach (var term in Dictionary)
                s.Append(term.Key);

            using (StreamWriter writer = new StreamWriter(path + "indexstring.txt"))
            {
                writer.Write(s.ToString());
            }
        }
        public void WriteCompressedStringToFile(string path)
        {

            byte k = 0;
            int blockReference = 0;
            byte blockLength = 0;
            string[] termsBlock = new string[4];
            string root = string.Empty;

            StringBuilder s = new StringBuilder();

            foreach (var term in Dictionary)
            {
                if (k < 4)
                {
                    termsBlock[k] = term.Key;

                    //s.Append(term.Key);
                    //blockLength += (byte)term.Key.Length;
                    //blockReference += blockLength;
                    k++;

                    //previousTerm = term.Key;
                }
                foreach (var t in termsBlock)
                {

                }
                else
                {
                    s.Append(blockLength);
                    blockReference += blockLength.ToString().Length;
                    CompressedDictionary.Add(term.Value, blockReference);

                    blockLength = 0;

                    k = 0;

                }
            }    //
            using (StreamWriter writer = new StreamWriter(path + "compressedindexstring.txt"))
            {
                writer.Write(s.ToString());
            }
        }
    }
}

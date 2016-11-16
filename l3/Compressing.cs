using System;
using System.Collections.Generic;
using Index;
using System.Text;
using System.IO;
using System.Linq;
using System.Collections;

namespace Compressing
{
    static class Compressor
    {
        public static BitArray LengthCode(BitArray input)
        {
            BitArray array = new BitArray(input.Length + 1);
            for (var i = 0; i < array.Length-1; i++) 
                array[i] = true;
            array[array.Length - 1] = false;
            return array;
        }
        public static BitArray ShiftCode(int n)
        {
            BitArray b = new BitArray(32);
            int pos = 31;
            int i = 0;
            int first1 = 0;
            while (i < 32)
            {
                if ((n & (1 << i)) != 0)
                {
                    b[pos] = true;
                    first1 = pos;
                }
                else
                {
                    b[pos] = false;
                }
                pos--;
                i++;
            }
            BitArray res = new BitArray(31-first1);
            for (int c = 0; c < (31-first1); c++)
            {
                var x = b[c + first1 + 1];
                res[c] = x;
            }
            return res;
        }
        public static string CompressedInvIndexToString(InvertedIndex index)
        {
            StringBuilder s = new StringBuilder();
            foreach (var term in index.Index)
            {
                if (term.Value.Count > 1)
                {
                    for (var i = 1; i < term.Value.Count; i++)
                    {
                        var gap = term.Value[i] - term.Value[i - 1];
                        foreach (var bit in LengthCode(ShiftCode(gap)).Cast<bool>())
                            s.Append(bit ? "1" : "0");
                        foreach (var bit in ShiftCode(gap).Cast<bool>())
                            s.Append(bit ? "1" : "0");
                    }
                 }
            }
            return s.ToString();
        }
        public static void WriteStringToFile(InvertedIndex index,string path)
        {
            StringBuilder s = new StringBuilder();
            foreach (var term in index.Index)
                s.Append(term.Key);
            using (StreamWriter writer = new StreamWriter(path + "dict.txt"))
            {
                writer.Write(s.ToString());
            }
        }
        public static string CompressedDictToString(InvertedIndex index)
        {
            byte k = 0;
            string[] termsBlock = new string[4];
            string root = string.Empty;

            StringBuilder s = new StringBuilder();

            foreach (var term in index.Index)
            {
                if (k < 4)
                {
                    termsBlock[k] = term.Key;
                    k++;
                }
                else
                {
                    var block = CompressedTermsBlock(termsBlock);
                    s.Append(block);
                    k = 0;
                }
            }
            return s.ToString();
        }

        private static string CompressedTermsBlock(string[] termsBlock)
        {
            var minLength = termsBlock.OrderBy(s => s.Length).First().Length;
            string root = string.Empty;
            for (var i = 0; i< minLength; i++)
            {
                if (Array.TrueForAll(termsBlock, t => t[i] == termsBlock[0][i]))
                {
                    root = root + termsBlock[0][i];
                }
            }
            StringBuilder result = new StringBuilder((root + termsBlock[0]).Substring(root.Length).Length + root + "*" + termsBlock[0].Substring(root.Length));
            for (var i = 1; i < termsBlock.Length; i++)
            {
                result.Append(termsBlock[i].Substring(root.Length).Length + "" + termsBlock[i].Substring(root.Length));
            }
            return result.ToString();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Index;
using System.Text.RegularExpressions;

namespace Search
{
    static class BoolSearch
    {
        // x AND y NOT z
        public static List<int> Search(string input, IIndex index)
        {
            var s = "Here is a test AND statement for AND regex";
            string pattern = @"\b(?=((\w+)\s+AND\s+(\w+))\b)";

            var andTerms = Regex.Matches(s, pattern)
                                            .Cast<Match>()
                                            .Select(m => m.Groups[0].Value + m.Groups[1].Value)
                                            .ToList();
            //var notTerm = Regex.Match(input, @"OR\s{1}(.+)").ToString();
            //var fst = index.GetValue(andTerms[0]);
            //var snd = index.GetValue(andTerms[1]);
            //var res = fst.Intersect(snd).Except(index.GetValue(notTerm)).ToList();
            //return res;
            return new List<int>();
        } 
    }
}

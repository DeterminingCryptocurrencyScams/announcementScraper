using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace AnnPostScraper.core.Extensions
{
   public static class StringExtensions
    {
        public static string RemoveGarbage(this string str)
        {
            var pattern = @"\t|\n|\r";
            str = Regex.Replace(str, pattern, "");

            str = str.Replace("  ", "");
            return str;
        }
        public static string RemoveEmojis(this string str)
        {
            // var pattern = @"[^\u0000-\u007F]+";
            // str = Regex.Replace(str, pattern, "");
            str = Regex.Replace(str, @"\p{Cs}", "");
            return str;
        }
    }
}

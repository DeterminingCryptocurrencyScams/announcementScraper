using System;
using System.Collections.Generic;
using System.Text;

namespace cryptoAnalysisScraper.core.crawler
{
    public class XpathSelectors
    {
        public static string baseSelector { get; set; } = "/html[1]/body[1]/div[2]/table[1]/tr[1]/td[1]/table[1]/tr[2]/td[1]/table[1]";
        public static string NameSelector { get; set; } = "/html[1]/body[1]/div[2]/table[1]/tr[1]/td[1]/table[1]/tr[2]/td[1]/table[1]/tr[1]/td[2]";
    }
}

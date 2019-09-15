using System;
using System.Collections.Generic;
using System.Text;

namespace AnnoucementScraper.core.crawler.Xpaths
{
    public class AnnCollectorXpaths
    {
        public static string BaseSelector { get; set; } = "/html[1]/body[1]/div[2]/div[2]/table[1]";
        public static string FirstPageBaseSelector { get; set; } = "/html[1]/body[1]/div[2]/div[3]/table[1]";
    }
}

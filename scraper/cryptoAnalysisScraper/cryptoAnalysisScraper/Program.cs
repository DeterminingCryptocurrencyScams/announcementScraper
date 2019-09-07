using cryptoAnalysisScraper.core.crawler;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;

namespace cryptoAnalysisScraper
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Program is starting");
            new UserCrawler().Scrape();
        }
    }
}

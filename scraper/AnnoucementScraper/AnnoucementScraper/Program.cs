using AnnoucementScraper.core.crawler;
using System;

namespace AnnoucementScraper
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Program is starting");
            new AnnTaskScraper().Scrape();
        }
    }
}

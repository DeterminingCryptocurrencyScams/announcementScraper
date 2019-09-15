using System;

namespace AnnPostScraper
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Program is starting");
            new AnnPostScraper.core.crawler.AnnPostScraper().Scrape();
        }
    }
}

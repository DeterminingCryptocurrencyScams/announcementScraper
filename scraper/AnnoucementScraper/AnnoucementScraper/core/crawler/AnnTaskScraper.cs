using AnnoucementScraper.core.crawler.Xpaths;
using AnnoucementScraper.core.database;
using AnnoucementScraper.core.models;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace AnnoucementScraper.core.crawler
{
    public class AnnTaskScraper
    {
        private const string BASE_URL = "https://bitcointalk.org/index.php?board=159";
        private System.Timers.Timer timer { get; set; } = new System.Timers.Timer();
        public int CurrentPage { get; set; } = 0;
        private const int End = 36920/40;
        public bool isRunning { get; set; }

        public void Scrape()
        {
            timer.Interval = 1000;
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
            isRunning = true;
            var random = new Random();
            while (isRunning)
            {
                Thread.Sleep(random.Next(10000));
            }
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            HtmlWeb web = new HtmlWeb();
            if (CurrentPage + 40 >= End)
            {
                //Game over boys!
                timer.Stop();
                isRunning = false;
            }
            var num = NextPage();
            var doc = web.Load(Url(num));

            Parse(doc, num);
        }
        private void Parse(HtmlDocument doc, int num)
        {
            HtmlNode baseCol;
            if (num > 0)
            {
                baseCol = doc.DocumentNode.SelectSingleNode(AnnCollectorXpaths.BaseSelector);
            }
            else
            {
                baseCol = doc.DocumentNode.SelectSingleNode(AnnCollectorXpaths.FirstPageBaseSelector);
            }

            if (baseCol == null)
            {
                throw new Exception("Base collection should never be null");
            }
            var tableRows = baseCol.Elements("tr");
            if (tableRows == null)
            {
                throw new Exception("Table Rows should never be null!");
            }
            for (int i = 1; i < tableRows.Count(); i++)
            {
                ParseRowAsync(tableRows.ElementAt(i));
            }

        }
        private async void ParseRowAsync(HtmlNode node)
        {
            if (!node.InnerHtml.ToLower().Contains("ann"))
            {
                //not an ann
                return;
            }
            var td = node.Elements("td");
            if (td == null)
            {
                throw new Exception("Could not find any tds as children!");
            }
            var model = HandleRow(td);
            var context = new MariaContext();
           // await context.AnnTasks.AddAsync(model);
           // await context.SaveChangesAsync();
            context.Dispose();
            return;
        }

        private AnnTaskModel HandleRow(IEnumerable<HtmlNode> nodes)
        {
            var model = new AnnTaskModel();
            for (int i = 2; i < nodes.Count(); i++)
            {
                
                var ele = nodes.ElementAt(i);
                switch (i)
                {
                    case (2): //title and url
                        var a = ele.Element("span").Element("a");
                        model.PostTitle = a.InnerText.Replace("�", "");
                        model.PostUrl = a.Attributes.Where(f=>f.Name == "href").FirstOrDefault().Value;
                        break;
                    case (3): // Profile
                        model.Author = ele.Element("a").InnerText.Replace("�","");
                        break;
                    case (4):
                        model.Replies = int.Parse(Regex.Replace(ele.Element("#text").InnerText, @"\t|\n|\r", ""));
                        break;
                    case (5):
                        model.Views = int.Parse(Regex.Replace(ele.Element("#text").InnerText, @"\t|\n|\r", ""));
                        break;
                    default:
                        break;
                }
            }

            return model;
        }
        private string Url(int num)
        {
            return $"{BASE_URL}.{num*40}";
        }
        private int NextPage()
        {
            var curr = this.CurrentPage;
            this.CurrentPage++;
            return curr;
        }
    }
}

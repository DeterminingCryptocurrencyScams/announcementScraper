using AnnoucementScraper.core.database;
using AnnoucementScraper.core.models;
using AnnPostScraper.core.crawler.xpath;
using AnnPostScraper.core.Extensions;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace AnnPostScraper.core.crawler
{
    public class AnnPostScraper
    {
        public System.Timers.Timer Timer { get; set; } = new System.Timers.Timer();
        public bool isRunning { get; set; }
        public int PageNumber { get; set; } = 0;
        public void Scrape()
        {
            Timer.Interval = 1000;
            Timer.Elapsed += Timer_Elapsed; ;
            Timer.Start();
            isRunning = true;
            var random = new Random();
            while (isRunning)
            {
                Thread.Sleep(random.Next(10000));
            }
        }

        private async void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            var c = new MariaContext();
            var task = await c.NextTask();
            c.Dispose();
            if (task == null)
            {
                return;
            }
            HtmlWeb web = new HtmlWeb();

            Parse(web.Load(MakeUrl(task.PostUrl)), task);
            PageNumber++;
        }
        private void Parse(HtmlDocument doc, AnnTaskModel task)
        {
            int postNumber = 0;
            var baseCol = doc.DocumentNode.SelectSingleNode(PostXpaths.BaseSelector);
            var tr = baseCol.Elements("tr");
            foreach (var row in tr)
            {
                postNumber++;
                ParsePost(row, doc, postNumber, task);
            }
        }
        public void ParsePost(HtmlNode node, HtmlDocument doc, int postNum, AnnTaskModel task)
        {
            if (node.InnerText.Contains("They may be unsafe, untrustworthy, or illegal in your jurisdiction.")) //ITS AN AD!
            {
                return;
            }
            var model = new PostModel(task);
            model.PostNumber = postNum;
            model.TopicTitle = model.TopicTitle.RemoveEmojis();
            var td = node.Element("td");
            if (td == null)
            {
                throw new Exception("Could not find any tds as children");
            }
            if (!td.HasChildNodes)
            {
                return;
            }
            var table = td.Element("table");

            if (table.ChildNodes.Any(f=>f.Name == "tbody"))
            {
                table = table.Element("tbody");
            }
            table = table.Element("tr").Element("td").Element("table");

            if (table.ChildNodes.Any(f => f.Name == "tbody"))
            {
                table = table.Element("tbody");
            }

            var details = table.Element("tr");
            
            GetDetails(details, model);
            GetPost(details, model);
            model.IsScamHeaderPresent = IsPossibleScam(doc);
            var context = new MariaContext();
            context.Posts.Add(model);
            context.SaveChanges();
            context.Dispose();
        }
        private void GetPost(HtmlNode details, PostModel model)
        {
            var body = details.Elements("td").ElementAt(1);

            var date = body.Element("table");
            if (date.ChildNodes.Any(f=>f.Name == "tbody"))
            {
                date = date.Element("tbody");
            }
            date = date.Element("tr").Elements("td").ElementAt(1).Elements("div").ElementAt(1);

            if (date.HasChildNodes)
            {
                var candidate = date.Element("span");
                if (candidate == null)
                {
                    date = date.Element("#text");
                }
                else
                {
                    date = candidate;
                }
            }
            model.PostedAt = date.InnerText;

            var div = body.Element("div");
            model.Body = div.InnerText.RemoveEmojis();
        }
        private PostModel GetDetails(HtmlNode details, PostModel model)
        {


            var details2 = details.Element("td");
            if (details2.ChildNodes.Count > 1)
            {
                details = details2;
            }
            model.Author = details.Element("b").InnerText;
            var userUrl = details.Element("b").Element("a").Attributes.Where(f => f.Name == "href").FirstOrDefault().Value;

            userUrl = userUrl.Replace("https://bitcointalk.org/index.php?action=profile;u=", "");

            model.UserId = int.Parse(userUrl);

            var div = details.Element("div");
            model.Position = div.Element("#text").InnerText.RemoveGarbage();

            var meritNode = div.ChildNodes.Where(f => f.InnerText.Contains("Merit"));
            string merit = "-1";
            if (!(meritNode == null))
            {
                if (meritNode.Count() != 0)
                {
                    merit = meritNode.FirstOrDefault().InnerText;

                }
            }

            merit.RemoveGarbage();
            if (merit.Contains(":"))
            {
            model.Merit = int.Parse(merit.Split(':')[1]);

            }
            else
            {
                model.Merit = int.Parse(merit);
            }

            var activityNode = div.ChildNodes.Where(f => f.InnerText.Contains("Activity"));
            var activity = "-1";
            if (!(activityNode == null))
            {
                if (activityNode.Count() != 0)
                {
                    activity = activityNode.FirstOrDefault().InnerText;
                }
            }

            activity.RemoveGarbage();
            if (activity.Contains(":"))
            {
            model.Activity = int.Parse(activity.Split(':')[1]);
            }
            else
            {
                model.Activity = int.Parse(activity);
            }

            return model;
        }

        private string MakeUrl(string url)
        {
            return $"{url}.{CurrentPage()}";
        }
        private int CurrentPage()
        {
            return PageNumber * 20;
        }
        private bool IsPossibleScam(HtmlDocument doc)
        {
            return doc.DocumentNode.InnerHtml.Contains("One or more bitcointalk.org users have reported that they strongly believe that the creator of this topic is a scammer.");
        }
    }
}

using AnnoucementScraper.core.database;
using AnnoucementScraper.core.models;
using AnnPostScraper.core.crawler.xpath;
using AnnPostScraper.core.Extensions;
using HtmlAgilityPack;
using Serilog;
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
        public AnnPostScraper()
        {
            var formatter = new Serilog.Formatting.Json.JsonFormatter();

            Log.Logger = new LoggerConfiguration().MinimumLevel.Debug().MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Information).Enrich.FromLogContext().WriteTo.Console().CreateLogger();
        }
        public System.Timers.Timer Timer { get; set; } = new System.Timers.Timer();
        public bool isRunning { get; set; }
        public bool CanGo { get; set; } = true;
        public int Inprogress { get; set; } = 0;
        public void Scrape()
        {
            Timer.Interval = 3400;
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

            if (CanGo == false)
            {
                CanGo = true;
            }
            if (Inprogress > 7)
            {
                return;
            }
            Inprogress++;
            var c = new MariaContext();
            var task = await c.NextTask();
            c.Dispose();
            if (task == null)
            {
                return;
            }
            HtmlWeb web = new HtmlWeb();

            Parse(web.Load(MakeUrl(task.PostUrl, 0)), task);
        }
        private void Parse(HtmlDocument doc, AnnTaskModel task)
        {
            var random = new Random();
            Log.Information($"Starting on task id:{task.Id}");
            int pageNumber = 1;
            int postNumber = 0;
            bool isWorking = true;

            int totalPages = GetRealTotalPages(doc.DocumentNode, task);
            Log.Information($"task {task.Id} has {totalPages} pages, which is PROJECTED to be {totalPages * 20} total posts");
            var baseCol = doc.DocumentNode.SelectSingleNode(PostXpaths.BaseSelector);
            while (isWorking)
            {
            var tr = baseCol.Elements("tr");
            foreach (var row in tr)
            {
                postNumber++;
                    Log.Information($"task {task.Id} scraping post number {postNumber} on page {pageNumber}");
                ParsePost(row, doc, postNumber, task);
            }


            if (totalPages <= pageNumber)
            {
                    Log.Information($"Finished scraping {task.Id} with {postNumber} posts.");
                    isWorking = false;
                    break;
            }
                pageNumber++;
                HtmlWeb web = new HtmlWeb();
                while (CanGo == false)
                {
                    Thread.Sleep(random.Next(4000));
                }
                CanGo = false;
                var page = web.Load(MakeUrl(task.PostUrl, pageNumber));
                if (page.DocumentNode.InnerText.Contains("you are accessing the forum too quickly"))
                {
                    Log.Error("uh oh, we are rate limited!!! Stoping the timer for 1 minute");

                    Timer.Stop();

                    Thread.Sleep(60000);
                    Log.Information("Resuming operations");
                    Timer.Start();
                }
                 baseCol =  page.DocumentNode.SelectSingleNode(PostXpaths.BaseSelector);
                if (baseCol == null)
                {
                    Log.Error($"BaseCol is nullllll");
                }
            }

            var c = new MariaContext();
            c.UpdateTaskStatusToComplete(task);
            Inprogress--;
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

        private int GetRealTotalPages(HtmlNode node, AnnTaskModel task)
        {
            var n = node.SelectSingleNode("/html[1]/body[1]/div[2]");

            var table = n.Element("table");
            if (table.ChildNodes.Any(f => f.Name == "tbody")) 
            {
                table = table.Element("tbody");
            }
            var t = table.Element("tr").Element("td");


            var allAs = t.Elements("a");

            if (allAs.Count() == 0)
            {
                return 0;
            }

            if (allAs.Last().InnerText.Contains("All"))
            {
                return int.Parse(allAs.ElementAt(allAs.Count() - 2).InnerText);
            }

            return int.Parse(allAs.ElementAt(allAs.Count() - 1).InnerText);
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

           merit = merit.RemoveGarbage();
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

        private string MakeUrl(string url, int pageNumber)
        {
            var usefulUrl = url.Remove(url.Length - 2);
            return $"{usefulUrl}.{pageNumber  * 20}";
        }
        private bool IsPossibleScam(HtmlDocument doc)
        {
            return doc.DocumentNode.InnerHtml.Contains("One or more bitcointalk.org users have reported that they strongly believe that the creator of this topic is a scammer.");
        }
    }
}

using cryptoAnalysisScraper.core.crawler.models;
using cryptoAnalysisScraper.core.database;
using cryptoAnalysisScraper.core.models;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace cryptoAnalysisScraper.core.crawler
{
    public class UserCrawler
    {
        private const string BASE_URL = "https://bitcointalk.org/index.php?action=profile;";
        private System.Timers.Timer timer { get; set; } = new System.Timers.Timer();
        public int End { get; set; } = 3000000;
        public bool isRunning { get; set; } = false;
        private bool isWorking { get; set; } = false;
        public void Scrape()
        {

            var formatter = new Serilog.Formatting.Json.JsonFormatter();
            Log.Logger = new LoggerConfiguration().MinimumLevel.Debug().MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Information).Enrich.FromLogContext().WriteTo.Console().CreateLogger();

            timer.Interval = 1000; //1 second
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
            isRunning = true;

            var random = new Random();

            while (isRunning)
            {
                  Thread.Sleep(random.Next(10000)); //keeps app alive
            }
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Log.Information("timer elapsed");

            var context = new MariaContext(); //reinstaniating like this means its thread safe
            var status = context.NextProfile();
            Log.Information("NextProfile finished successfully");
            if (status == null)
            {
                context.Dispose();
                return;
            }
            if (status.Id < End)
            {
                isWorking = true;
                HtmlWeb web = new HtmlWeb();

                var doc = web.Load(MakeUrl(status.Id));
                Log.Information("Loaded page successfully");
                var result = this.Parse(status.Id, doc, status);
                Log.Information("parsed successfully");

                if (result != null)
                {
                    status.Status = core.models.ProfileStatus.Complete;
                    context.SetStatusForId(status);
                   context.Users.Add(result); 
                    context.SaveChanges();
                }
                else
                {
                    status.Status = core.models.ProfileStatus.ProfileNotPresent;
                    context.SetStatusForId(status);
                }
            }
            else
            {
                Log.Information("oh no, in a bad place! <- could be the root cause!");

                timer.Stop();
                isRunning = false;
            }
            context.Dispose();
        }

        private UserPageModel Parse(int id,HtmlDocument doc, UserProfileScrapingStatus userProfileStatus)
        {
            if (doc.DocumentNode.InnerHtml.Contains("An Error Has Occurred!")){
                Log.Information("parsing - Profile doesn't exist");

                return null;
            }
            if (doc.DocumentNode.InnerText.Contains("403"))
            {
                Log.Information("rate limited!!!! <- could be the root cause!");
                var context = new MariaContext();
                userProfileStatus.Status = ProfileStatus.Error;
                context.SetStatusForId(userProfileStatus);
                context.Dispose();
                throw new Exception("Error! getting 403 response. Quitting so we don't get locked out for longer!");
            }

            var item = new UserPageModel(id);
            item.Name = handleItem(doc.DocumentNode.SelectNodes(XpathSelectors.NameSelector));
            var baseCol = doc.DocumentNode.SelectSingleNode(XpathSelectors.baseSelector);
            if (baseCol == null)
            {
                throw new Exception("Error, should never be null!");
            }
            
            item.Merit = handleItem(baseCol.SelectNodes($"{DynamicXpath(baseCol,"Merit")}/td[2]"));
            item.Position = handleItem(baseCol.SelectNodes($"{DynamicXpath(baseCol, "Position")}/td[2]"));
            item.Posts = handleItem(baseCol.SelectNodes($"{DynamicXpath(baseCol, "Posts")}/td[2]"));
            item.Activity = handleItem(baseCol.SelectNodes($"{DynamicXpath(baseCol, "Activity")}/td[2]"));
            item.DateRegistered = handleItem(baseCol.SelectNodes($"{DynamicXpath(baseCol, "Date Registered")}/td[2]"));
            item.LastActive = handleItem(baseCol.SelectNodes($"{DynamicXpath(baseCol, "Last Active")}/td[2]"));
            item.Gender = handleItem(baseCol.SelectNodes($"{DynamicXpath(baseCol, "Gender")}/td[2]"));
            item.Age = handleItem(baseCol.SelectNodes($"{DynamicXpath(baseCol, "Age")}/td[2]"));
            item.Location = handleItem(baseCol.SelectNodes($"{DynamicXpath(baseCol, "Location")}/td[2]"));
            item.LocalTime = handleItem(baseCol.SelectNodes($"{DynamicXpath(baseCol, "Local Time")}/td[2]"));
            Log.Information("Finished successfully");

            return item;
        }
        private string handleItem(HtmlNodeCollection col)
        {
            if (col == null)
            {
                Log.Information("collection is null, returning empty string");

                return "";
            }
            else if (col.FirstOrDefault() != null)
            {
                Log.Information("returning col text");

                return col.FirstOrDefault().InnerText;
            }
            else
            {
                return "";
            }
        }
        private string DynamicXpath(HtmlNode col, string searchingFor)
        {
           var node = col.ChildNodes.Where(f => f.InnerText.Contains(searchingFor)).FirstOrDefault();
            Log.Information($"Node was searched for: {searchingFor}, found: {node}");

            return node.XPath;

        }
        private string MakeUrl(int id)
        {
            return $"{BASE_URL}u={id}";
        }
    }
}

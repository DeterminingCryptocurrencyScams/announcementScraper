using cryptoAnalysisScraper.core.crawler.models;
using cryptoAnalysisScraper.core.models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
namespace cryptoAnalysisScraper.core.database
{
   public class MariaContext : DbContext
    {
        
        public MariaContext()
        {
            var formatter = new Serilog.Formatting.Json.JsonFormatter();
            
            Log.Logger = new LoggerConfiguration().MinimumLevel.Debug().MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Information).Enrich.FromLogContext().WriteTo.Console().CreateLogger();
        }
        public DbSet<UserPageModel> Users { get; set; }
        public DbSet<UserProfileScrapingStatus> ProfileScrapingStatuses { get; set; }
        public MariaContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MariaContext>();
            optionsBuilder.UseMySql(@"Server=database-1.c0srsxgmo39w.us-east-2.rds.amazonaws.com;User Id=scraper2;Database=innodb");
            return new MariaContext();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder o)
        {
            o.UseMySql(@"Server=database-1.c0srsxgmo39w.us-east-2.rds.amazonaws.com;User Id=scraper2;Database=innodb");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserProfileScrapingStatus>().Property(f => f.Id).ValueGeneratedOnAdd();
        }
        public UserProfileScrapingStatus NextProfile()
        {
            Log.Logger.Information("In next Profile");
            var s = new UserProfileScrapingStatus();
            this.ProfileScrapingStatuses.Add(s);
            Log.Logger.Information("adding s to profileScrapingStatus");

            this.SaveChanges();
            Log.Logger.Information($"saved changes, Starting work on {s.Id}");
            return s;
        }
        public bool SetStatusForId(int id, ProfileStatus status)
        {
            return SetStatusForId(new UserProfileScrapingStatus(id, status));
        }
        public bool SetStatusForId(UserProfileScrapingStatus status)
        {
            try
            {
                this.Entry(status).State = ProfileStatusExists(status.Id) ? EntityState.Modified : EntityState.Added;
                this.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                throw e;
            }
        }
        private bool ProfileStatusExists(int id)
        {
            return this.ProfileScrapingStatuses.Any(e => e.Id == id);
        }
    }
    
}

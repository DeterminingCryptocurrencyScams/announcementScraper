using AnnoucementScraper.core.models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnnoucementScraper.core.database
{
   public class MariaContext: DbContext
    {

        public DbSet<AnnTaskModel> AnnTasks { get; set; }
        public DbSet<PostModel> Posts { get; set; }
       public MariaContext CreateDbContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<MariaContext>();
            optionsBuilder.UseMySql(@"Server=database-1.c0srsxgmo39w.us-east-2.rds.amazonaws.com;User Id=scraper2;Database=innodb");
            return new MariaContext();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder o)
        {
            o.UseMySql(@"Server=database-1.c0srsxgmo39w.us-east-2.rds.amazonaws.com;User Id=scraper;Database=innodb");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AnnTaskModel>().Property(f => f.Id).ValueGeneratedOnAdd();
        }

        public async Task<AnnTaskModel> NextTask()
        {
            var possibleTasks = this.AnnTasks.Where(f => f.Status == AnnStatus.Waiting);
            var r = new Random();
            var total = possibleTasks.Count();
            if (total == 0)
            {
                return null;
            }
            var randomInt = r.Next(0, total);
            var task = possibleTasks.AsEnumerable().ElementAt(randomInt);
            task.Status = AnnStatus.Working;
            //this.Entry(task).State = EntityState.Modified;
            //this.SaveChanges();
            return task;
        }
    }
}

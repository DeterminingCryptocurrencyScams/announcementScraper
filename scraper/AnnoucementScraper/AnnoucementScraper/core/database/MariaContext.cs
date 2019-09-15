using AnnoucementScraper.core.models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnnoucementScraper.core.database
{
   public class MariaContext: DbContext
    {
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

        public DbSet<AnnTaskModel> AnnTasks { get; set; }
    }
}

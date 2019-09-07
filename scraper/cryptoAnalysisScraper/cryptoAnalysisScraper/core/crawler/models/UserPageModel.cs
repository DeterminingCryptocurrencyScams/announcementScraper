using System;
using System.Collections.Generic;
using System.Text;

namespace cryptoAnalysisScraper.core.crawler.models
{
   public class UserPageModel
    {
        public UserPageModel(int id)
        {
            Id = id;
            this.RetreivedAt = DateTime.Now;
        }
        public int Id { get; set; }
        public string Name{ get; set; }
        public string Posts{ get; set; }
        public string Activity { get; set; }
        public string Merit { get; set; }
        public string Position { get; set; }
        public string DateRegistered { get; set; }
        public string LastActive { get; set; }
        public string Gender { get; set; }
        public string Age { get; set; }
        public string Location { get; set; }
        public string LocalTime { get; set; } //We can use this and the below prop to figure out an approximate timezone for the user
        public DateTime RetreivedAt { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace AnnoucementScraper.core.models
{
   public class AnnTaskModel
    {
        public int Id { get; set; }
        public string PostTitle { get; set; }
        public string PostUrl { get; set; }
        public string Author { get; set; }
        public int Views { get; set; }
        public int Replies { get; set; }
        public AnnStatus Status { get; set; } = AnnStatus.Waiting;
        public DateTime RetrievedAt { get; set; } = DateTime.Now;
        public DateTime? TaskStartedAt { get; set; } = null;
    }
    public enum AnnStatus
    {
        Unknown = 0,
        Waiting = 1,
        Working = 2,
        Complete = 3,
        Error = 4,
    }
}

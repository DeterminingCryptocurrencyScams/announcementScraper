using System;
using System.Collections.Generic;
using System.Text;

namespace AnnoucementScraper.core.models
{
   public class PostModel
    {
        public PostModel() { }
        public PostModel(AnnTaskModel annTask)
        {
            this.TopicAuthor = annTask.Author;
            this.TopicTitle = annTask.PostTitle;
            this.TopicUrl = annTask.PostUrl;
            this.TaskId = annTask.Id;
        }

        public Guid Id { get; set; } = Guid.NewGuid();
        public string TopicTitle { get; set; }
        public string TopicUrl { get; set; }
        public string TopicAuthor { get; set; }
        public string Author { get; set; }
        public int UserId { get; set; }
        public int Merit { get; set; }
        public int Activity { get; set; }
        public string Position { get; set; }
        public string Body { get; set; }
        public int TaskId { get; set; }
        public int PostNumber { get; set; }
        public bool IsScamHeaderPresent { get; set; }
        public string PostedAt { get; set; }
        public DateTime RetrievedAt { get; set; } = DateTime.Now;
        public PostStatus Status { get; set; } = PostStatus.Working;
    }
    public enum PostStatus
    {
        Unknown = 0,
        Waiting = 1,
        Working = 2,
        Complete = 3,
        Error = 4,
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace cryptoAnalysisScraper.core.models
{
    public class UserProfileScrapingStatus
    {
        public UserProfileScrapingStatus(int id, ProfileStatus status)
        {
            Id = id;
            Status = status;
        }
        public UserProfileScrapingStatus()
        {
            Status = ProfileStatus.Working;
        }
        public int Id { get; set; }
        public ProfileStatus Status { get; set; }
    }
   public enum ProfileStatus
    {
        Unknown = 0,
        Working = 1,
        Complete = 2,
        ProfileNotPresent = 3,
        Error = 4,
    }
}

using System;

namespace _20LHWebPortal.Models.Events
{
    public class EventCard
    {
        public long EventId { get; set; }
        public string Title { get; set; }
        public string BannerImageUrl { get; set; }
        public string UserImageUrl { get; set; }
        public string UserFullName { get; set; } //First & Last name
        public string Description { get; set; } //120 max length!
        public string Location { get; set; } //City state for right now
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
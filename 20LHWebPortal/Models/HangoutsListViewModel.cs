using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _20LHWebPortal.Models
{
    public class HangoutsListViewModel
    {
        public string UserGender { get; set; }
        public List<HangoutViewModel> ListOfHangouts { get; set; }

        public HangoutsListViewModel()
        {
            ListOfHangouts = new List<HangoutViewModel>();
        }
    }

    





    public class HangoutViewModel
    {
        
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? Date  { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public int OpenSpots { get; set; }
        public bool IsRsvp { get; set; }
        public bool IsHost { get; set; }

        public string HostName { get; set; }
        public UserViewModel HostUser { get; set; }
        
        public string Location { get; set; }


        public int MaleOpenSpots { get; set; }

        public int FemaleOpenSpots { get; set; }

        public bool GenderRatio { get; set; }
        public List<UserViewModel> AttendingList { get; set; }

        public double? HostAverageRating { get; set; }

        public double? HangoutAverageRating { get; set; }

        public byte[] ImageContent { get; set; }

        public string ImageMimeType { get; set; }

        public HangoutViewModel ()
        {
            AttendingList = new List<UserViewModel>();
            ImageContent = new byte[8];
        }

    }
}
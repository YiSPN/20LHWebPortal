using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace _20LHWebPortal.Models
{
    public class RateOrganizerHangoutViewModel : CreateHangoutViewModel
    {
        public int HangoutRating { get; set; }

        [Display(Name = "Organizer Rating")]
        public int OrganizerRating { get; set; }


        public string AttendeeId { get; set; }

        public UserViewModel HostUser { get; set; }

        public List<UserViewModel> AttendingList { get; set; }
        
        public int OpenSpots { get; set; }

        public RateOrganizerHangoutViewModel()
        {
            AttendingList = new List<UserViewModel>();
            ImageContent = new byte[8];
        }
    }

    
}
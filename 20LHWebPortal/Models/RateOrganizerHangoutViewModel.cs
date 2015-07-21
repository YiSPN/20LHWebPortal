using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _20LHWebPortal.Models
{
    public class RateOrganizerHangoutViewModel : CreateHangoutViewModel
    {
        public int HangoutRating { get; set; }

        public int OrganizerRating { get; set; }

        public string AttendeeId { get; set; }

        public RateOrganizerHangoutViewModel()
        {

        }
    }

    
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _20LHWebPortal.Models
{
    public class UserViewModel
    {
        public string Name { get; set; }

        public double UserRating { get; set; }

        public string Gender { get; set; }

        public int NoShows { get; set; }

        public int HangoutsHosted { get; set; }

        public int HangoutsAttended { get; set; }

    }
}
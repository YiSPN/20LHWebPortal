using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _20LHWebPortal.Models
{
    public class ShowNoShowHangoutViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<ShowOrNoShowUser> UserList { get; set; }

        public ShowNoShowHangoutViewModel()
        {
            UserList = new List<ShowOrNoShowUser>();
        }
    }

    public class ShowOrNoShowUser
    {
        public string Username { get; set; }

        public string AttendeeId { get; set; }

        public bool Showed { get; set;}
    }
}
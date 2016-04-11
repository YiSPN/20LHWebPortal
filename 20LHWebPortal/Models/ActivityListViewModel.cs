using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _20LHWebPortal.Models
{
    public class ActivityListViewModel
    {
        public List<ActivityViewModel> ListOfActivities { get; set; }

        public ActivityListViewModel()
        {
            ListOfActivities = new List<ActivityViewModel>();
        }
    }

    public class ActivityViewModel
    {
        public string name { get; set; }

        public string hangoutName { get; set; }

        public DateTime timeStamp { get; set; }

        public int activityType { get; set; }
    }
        
}
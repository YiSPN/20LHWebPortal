using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using _20LHWebPortal.Models.Events;

namespace _20LHWebPortal.Controllers
{
    [RequireHttps]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //return RedirectToAction("Upcoming", "Hangout");
            return View();

        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult ThingsToDo()
        {
            return View();
        }

        public ActionResult FAQ()
        {
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult TestView()
        {
            return View();
        }

        public ActionResult Ui()
        {
            var model = new EventCard
            {
                EventId = 0,
                Title = "Pickup Soccer",
                Description = "Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua",
                BannerImageUrl = "/Content/Images/event-banner.jpg",
                Location = "Columbus, OH",
                UserFullName = "John Doe",
                UserImageUrl = "/Content/Images/user.png",
                StartTime = DateTime.Now.AddDays(10),
                EndTime = DateTime.Now.AddDays(10).AddHours(3)
            };
            return View(model);
        }
    }
}
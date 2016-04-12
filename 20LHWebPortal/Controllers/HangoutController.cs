using _20LHWebPortal.Models;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace _20LHWebPortal.Controllers
{
    [Authorize]
    public class HangoutController : BaseController
    {
        private IHangoutRepository _hangoutRepository;

        public HangoutController() : this(new HangoutRepository())
        {

        }

        public HangoutController(IHangoutRepository hangoutRepository)
        {
            _hangoutRepository = hangoutRepository;
        }

        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            var userInfo = new UserViewModel();
            userInfo.userRating = _hangoutRepository.GetUserRating(userId);
            userInfo.gender = _hangoutRepository.GetUserGender(userId);
            userInfo.username = _hangoutRepository.GetUserName(userId);
            userInfo.strikes = _hangoutRepository.GetStrikeCount(userId);
            return View(userInfo);
        }

         


        public ActionResult MyHangouts()
        {
            var userInfo = User.Identity.GetUserId();
            var hangouts = _hangoutRepository.ListMyHangouts(userInfo);
            var listViewModel = new HangoutsListViewModel
            {
                ListOfHangouts = hangouts
            };

            //Test comment
            //test commit 2

            //Map to view model

            ViewBag.Strikes = _hangoutRepository.GetStrikeCount(userInfo);
            return View(listViewModel.ListOfHangouts.AsQueryable());
        }

        public ActionResult Upcoming()
        {
            var userInfo = User.Identity.GetUserId();
            var hangouts = _hangoutRepository.ListUpcomingHangouts(userInfo);
            
            var listViewModel = new HangoutsListViewModel
            {
                ListOfHangouts = hangouts
            };
            listViewModel.UserGender = _hangoutRepository.GetUserGender(userInfo);

            //Map to view model
            ViewBag.Strikes = _hangoutRepository.GetStrikeCount(userInfo);
            return View(listViewModel);
        }

        public ActionResult Past()
        {

            var userInfo = User.Identity.GetUserId();
            var hangouts = _hangoutRepository.ListPastHangouts(userInfo);
            var listViewModel = new HangoutsListViewModel
            {
                ListOfHangouts = hangouts
            };

            //Map to view model

            ViewBag.Strikes = _hangoutRepository.GetStrikeCount(userInfo);
            return View(listViewModel.ListOfHangouts.AsQueryable());

            
        }

        public ActionResult Activity()
        {
            var userInfo = User.Identity.GetUserId();
            var activityListViewModel = new ActivityListViewModel
            {
                ListOfActivities = _hangoutRepository.ListActivityLog()
            };

            ViewBag.Strikes = _hangoutRepository.GetStrikeCount(userInfo);
            return View(activityListViewModel.ListOfActivities.AsQueryable());
        }

        public ActionResult Create()
        {
            return View();
        }

        public ActionResult Edit(int id)
        {
            var item = _hangoutRepository.GetHangoutById(id);
            var model = new CreateHangoutViewModel
            {
                Id = item.Id,
                Date = item.Date,
                Description = item.Description,
                Name = item.Name,
                UserId = item.UserCreator,
                Location = item.Location,
                Address = item.Address,
                ContactInfo = item.ContactInfo,
                PartySize = item.PartySize,
                GenderRatio = item.GenderRatio,
                StartTime = item.StartTime,
                EndTime = item.EndTime

            };
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CreateHangoutViewModel model)
        {
            JsonResult result = null;
            var errors = new List<string>();

/*            bool startTimeIsValid, endTimeIsValid;
            startTimeIsValid = IsValidTime(model.StartTime);
            endTimeIsValid = IsValidTime(model.EndTime);*/
            if (!model.StartTime.HasValue)
            {
                ModelState.AddModelError(string.Empty, model.StartTime + " is an invalid time. E.g. 10:00am");
                errors.Add(model.StartTime + " is an invalid time. E.g. 10:00am");
            }
            if (!model.EndTime.HasValue)
            {
                ModelState.AddModelError(string.Empty, model.EndTime + " is an invalid time. E.g. 3:00pm");
                errors.Add(model.EndTime + " is an invalid time. E.g. 3:00pm");
            }
            if(!model.Date.HasValue || (model.Date.Value.Date < DateTime.Today))
            {
                ModelState.AddModelError(string.Empty, "Date cannot be paste due.");
                errors.Add("Date cannot be paste due.");
            }

            if (!ModelState.IsValid && (model.StartTime.GetValueOrDefault() < model.EndTime.GetValueOrDefault()))
            {
                ModelState.AddModelError(string.Empty, "End Time must be later than Start Time");
                errors.Add("End Time must be later than Start Time");
            }

            var errorList = ModelState.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToList());

            if (!ModelState.IsValid || errors.Any())
            {
                foreach (var error in errorList)
                {
                    for (var i = 0; i < error.Value.Count; i++)
                    {
                        var message = string.Format("{0}: {1}", error.Key, error.Value[i]);
                        errors.Add(message);
                    }
                }
                result = new JsonResult
                {
                    Data = new
                    {
                        Errors = errors
                    }
                };
                return result;
            }
            else
            {
                _hangoutRepository.Update(model);

                var userId = User.Identity.GetUserId();
                var hangoutViewModel = _hangoutRepository.GetHangoutViewModelById(userId, model.Id);

                result = new JsonResult
                {
                    Data = new
                    {
                        Errors = hangoutViewModel == null ? new List<string> {"Unable to find the hangout"} : new List<string>(),
                        View = hangoutViewModel != null ? ViewToString("_UpcomingHangoutPartial", hangoutViewModel) : ""
                    }
                };
                return result;
            }


            //if (ModelState.IsValid && !errorList.Any())
            //{
            //    _hangoutRepository.Update(model);
            //    return RedirectToAction("MyHangouts");

                //var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                //var result = await UserManager.CreateAsync(user, model.Password);
                //if (result.Succeeded)
                //{
                //    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                //    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                //    // Send an email with this link
                //    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                //    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                //    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                //    return RedirectToAction("Index", "Home");
                //}
                //AddErrors(result);
            //}

            // If we got this far, something failed, redisplay form
            //return View(model);
        }

        public bool IsValidTime(string time)
        {
            DateTime outTime;
            return (DateTime.TryParseExact(time, "h:mmtt", CultureInfo.InvariantCulture, DateTimeStyles.AllowInnerWhite, out outTime));
        }

        public ActionResult Rate(int id)
        {
            var item = _hangoutRepository.GetHangoutById(id);
            var model = new RateOrganizerHangoutViewModel
            {
                Id = item.Id,
                Date = item.Date,
                Description = item.Description,
                Name = item.Name,
                UserId = item.UserCreator,
                Location = item.Location,
                Address = item.Address,
                ContactInfo = item.ContactInfo,
                PartySize = item.PartySize,
                GenderRatio = item.GenderRatio

            };
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Rate(RateOrganizerHangoutViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Grab Host Id and HangoutId from model
                // Put rating values in new model "RateHangoutViewModel"
                // _hangoutRepository.Rate(model);
              
                
                var userInfo = User.Identity.GetUserId();
                model.AttendeeId = userInfo;
                _hangoutRepository.RateOrganizerAndHangout(model);

                return RedirectToAction("MyHangouts");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult ShowNoShow(int id)
        {
            var model = _hangoutRepository.GetShowNoShow(id);
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ShowNoShow(ShowNoShowHangoutViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userInfo = User.Identity.GetUserId();
                //model.AttendeeId = userInfo;
                _hangoutRepository.ShoworNoShowSubmit(model);

                return RedirectToAction("Past");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateHangoutViewModel model)
        {
            /*bool startTimeIsValid, endTimeIsValid;
            startTimeIsValid = IsValidTime(model.StartTime);
            endTimeIsValid = IsValidTime(model.EndTime);*/


            if (!model.StartTime.HasValue)
            {
                ModelState.AddModelError(string.Empty, model.StartTime + " is an invalid time. E.g. 10:00am");
            }
            if (!model.EndTime.HasValue)
            {
                ModelState.AddModelError(string.Empty, model.EndTime + " is an invalid time. E.g. 3:00pm");
            }

            if (!model.Date.HasValue)
            {
                ModelState.AddModelError("Date", "Date cannot be empty.");
            }
            if (model.Date < DateTime.Today)
            {
                ModelState.AddModelError("Date", "Date cannot be paste due.");
            }
            if (model.StartTime > model.EndTime)
            {
                ModelState.AddModelError(string.Empty, "End Time must be later than Start Time");
            }

            if (ModelState.IsValid)
            {
                var userInfo = User.Identity.GetUserId();
                model.UserId = userInfo;
                _hangoutRepository.Create(model);
                
                
                return RedirectToAction("MyHangouts");

                //var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                //var result = await UserManager.CreateAsync(user, model.Password);
                //if (result.Succeeded)
                //{
                //    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                //    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                //    // Send an email with this link
                //    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                //    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                //    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                //    return RedirectToAction("Index", "Home");
                //}
                //AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        
        [AllowAnonymous]
        public async Task<ActionResult> Delete(int id)
        {
            _hangoutRepository.Delete(id);
            return RedirectToAction("MyHangouts");

            var testList = new List<int>();
            var count = testList.Count;
        }

        [AllowAnonymous]
        public async Task<ActionResult> Cancel(int id)
        {
            _hangoutRepository.Cancel(id);
            return RedirectToAction("MyHangouts");

            var testList = new List<int>();
            var count = testList.Count;
        }

        public async Task<ActionResult> Join(int id)
        {
            var userInfo = User.Identity.GetUserId();
            _hangoutRepository.JoinHangout(userInfo, id);
            return RedirectToAction("Upcoming");
        }

        public async Task<ActionResult> Waitlist(int id)
        {
            var userInfo = User.Identity.GetUserId();
            _hangoutRepository.WaitlistHangout(userInfo, id);
            return RedirectToAction("Upcoming");
        }

        public async Task<ActionResult> UnRSVP(int id)
        {
            var userInfo = User.Identity.GetUserId();
            _hangoutRepository.LeaveHangout(userInfo, id);
            return RedirectToAction("Upcoming");
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }
        

    }

    
}
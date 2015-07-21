using _20LHWebPortal.Models;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace _20LHWebPortal.Controllers
{
    [Authorize]
    public class HangoutController : Controller
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

            //Map to view model
           
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

            return View(listViewModel.ListOfHangouts.AsQueryable());

            
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
                Duration = item.Duration,
                PartySize = item.PartySize,
                GenderRatio = item.GenderRatio
                
            };
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CreateHangoutViewModel model)
        {
            if (ModelState.IsValid)
            {
                _hangoutRepository.Update(model);

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
                Duration = item.Duration,
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



        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateHangoutViewModel model)
        {
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
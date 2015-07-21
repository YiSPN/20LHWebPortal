﻿using _20LHWebPortal.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _20LHWebPortal.Models
{
    public interface IHangoutRepository
    {
        List<HangoutViewModel> ListMyHangouts(string userId);

        List<HangoutViewModel> ListUpcomingHangouts(string userId);

        List<HangoutViewModel> ListPastHangouts(string userId);
        void Create(CreateHangoutViewModel model);

        void JoinHangout(string userId, int hangoutId);
        void LeaveHangout(string userId, int hangoutId);

        void RateOrganizerAndHangout(RateOrganizerHangoutViewModel model);

        string GetUserGender(string userId);

        string GetUserName(string userId);

        void Delete(int id);

        Hangout GetHangoutById(int id);

        void Update(CreateHangoutViewModel model);

        void WaitlistHangout(string userId, int hangoutId);

        double GetUserRating(string userId);

    }
}

﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace _20LHWebPortal.Models
{
    public class HangoutRepository : IHangoutRepository
    {
        private HangoutDataContext Hangout_db;
        private AspNetUsers_HangoutDataContext AspNetUsers_Hangout_db;
        private AspNetUsersDataContext AspNetUsers_db;
        private OrganizerRatingsDataContext OrganizerRatings_db;
        private HangoutRatingsDataContext HangoutRatings_db;
        private ActivityLogDataContext ActivityLog_db;

        public HangoutRepository()
        {
            Hangout_db = new HangoutDataContext();
            AspNetUsers_Hangout_db = new AspNetUsers_HangoutDataContext();
            AspNetUsers_db = new AspNetUsersDataContext();
            OrganizerRatings_db = new OrganizerRatingsDataContext();
            HangoutRatings_db = new HangoutRatingsDataContext();
            ActivityLog_db = new ActivityLogDataContext();
        }
        public List<HangoutViewModel> ListMyHangouts(string userId)
        {
            var hangoutsAttending = from m in AspNetUsers_Hangout_db.AspNetUsers_Hangouts
                                    where m.AspNetUsers == userId && m.IsRSVPd == true || m.IsWaitlist == true
                                    select m;
            var myHangouts = from m in Hangout_db.Hangouts
                             where m.UserCreator == userId && new DateTime(m.Date.Value.Year, m.Date.Value.Month, m.Date.Value.Day, m.StartTime.Value.Hour, m.StartTime.Value.Minute, m.StartTime.Value.Second)  > DateTime.Now
                             select m;

            var returnList = new List<HangoutViewModel>();
            foreach(var h in myHangouts.ToList())
            {

                var allAtendees = (from u in AspNetUsers_Hangout_db.AspNetUsers_Hangouts
                                   where u.HangoutId == h.Id && u.IsRSVPd == true
                                   select u);

                var hostAverageRating = (from u in OrganizerRatings_db.OrganizerRatings
                                         where u.OrganizerId == h.UserCreator
                                         select u);
                double sum = 0;

                foreach (var r in hostAverageRating)
                {
                    sum += (double)r.Rating;
                }

                // Use aggregate in future...it wasnt working before.

                var average = sum / hostAverageRating.Count();


                var hangout = new HangoutViewModel
                {
                    Date = h.Date,
                    StartTime = h.StartTime,
                    EndTime = h.EndTime,
                    Description = h.Description,
                    Id = h.Id,
                    Name = h.Name,
                    OpenSpots = h.PartySize - allAtendees.Count(),
                    HostName = GetUserName(h.UserCreator),
                    IsHost = true,
                    IsRsvp = false,
                    HostAverageRating = Math.Round(average, 2),
                    Location = h.Location
                };

                foreach (var a in allAtendees)
                {
                    var user = (from y in AspNetUsers_db.AspNetUsers
                                where y.Id == a.AspNetUsers
                                select y).SingleOrDefault();
                    hangout.AttendingList.Add(new UserViewModel
                    {
                        username = user.UserName
                    });
                }
                returnList.Add(hangout);
            }
            // Map to returnList
            if(hangoutsAttending.Any())
            {
                foreach(var h in hangoutsAttending)
                {
                    var tempHangout = (from a in Hangout_db.Hangouts
                                       where a.Id == h.HangoutId && new DateTime(a.Date.Value.Year, a.Date.Value.Month, a.Date.Value.Day, a.StartTime.Value.Hour, a.StartTime.Value.Minute, a.StartTime.Value.Second) > DateTime.Now
                                   select a).SingleOrDefault();
                    if(tempHangout != null)
                    {
                        var allAtendees = (from u in AspNetUsers_Hangout_db.AspNetUsers_Hangouts
                                           where u.HangoutId == tempHangout.Id && u.IsRSVPd == true
                                           select u);

                        var hostAverageRating = (from u in OrganizerRatings_db.OrganizerRatings
                                                 where u.OrganizerId == tempHangout.UserCreator
                                                 select u);
                        double sum = 0;

                        foreach (var r in hostAverageRating)
                        {
                            sum += (double)r.Rating;
                        }

                        // Use aggregate in future...it wasnt working before.

                        var average = sum / hostAverageRating.Count();
                        var hangout = new HangoutViewModel
                        {
                            Date = tempHangout.Date,
                            Description = tempHangout.Description,
                            Id = tempHangout.Id,
                            Name = tempHangout.Name,
                            OpenSpots = tempHangout.PartySize - allAtendees.Count(),
                            HostName = GetUserName(tempHangout.UserCreator),
                            HostAverageRating = Math.Round(average, 2),
                            IsRsvp = true,
                            Location = tempHangout.Location
                        };

                        foreach (var a in allAtendees)
                        {
                            var user = (from y in AspNetUsers_db.AspNetUsers
                                        where y.Id == a.AspNetUsers
                                        select y).SingleOrDefault();
                            hangout.AttendingList.Add(new UserViewModel
                            {
                                username = user.UserName
                            });
                        }
                        returnList.Add(hangout);
                    }
                }
             }

            return returnList;
        }

        public List<ActivityViewModel> ListActivityLog()
        {
            var returnList = new List<ActivityViewModel>();
            var activities = from m in ActivityLog_db.ActivityLogs
                             orderby m.TimeStamp descending
                                    select m;
            foreach (var a in activities.ToList())
            {
                var activity = new ActivityViewModel
                {
                    activityType = a.ActivityType,
                    hangoutName = GetHangoutById(a.HangoutId).Name,
                    timeStamp = a.TimeStamp,
                    username = GetUserName(a.AspNetUserId)
                };
                returnList.Add(activity);
            }
            return returnList;
        }


        public List<HangoutViewModel> ListUpcomingHangouts(string userId)
        {
            
            var HangoutsAttending = from m in AspNetUsers_Hangout_db.AspNetUsers_Hangouts
                                    where m.AspNetUsers == userId && m.IsRSVPd == true || m.IsWaitlist == true
                                    select m;
            var myHangouts = from m in Hangout_db.Hangouts
                             where m.UserCreator == userId && new DateTime(m.Date.Value.Year, m.Date.Value.Month, m.Date.Value.Day, m.StartTime.Value.Hour, m.StartTime.Value.Minute, m.StartTime.Value.Second) > DateTime.Now
                             select m;
            var allHangouts = from m in Hangout_db.Hangouts
                             where new DateTime(m.Date.Value.Year, m.Date.Value.Month, m.Date.Value.Day, m.StartTime.Value.Hour, m.StartTime.Value.Minute, m.StartTime.Value.Second) > DateTime.Now
                             select m;
            var hangoutsGoing = new List<int>();
            foreach (var h in HangoutsAttending)
            {
                hangoutsGoing.Add(h.HangoutId);
            }
            foreach (var h in myHangouts)
            {
                hangoutsGoing.Add(h.Id);
            }

            var returnList = new List<HangoutViewModel>();
            foreach(var h in allHangouts)
            {
                if(!(hangoutsGoing.Contains(h.Id)))
                {
                    var tempHangout = (from a in Hangout_db.Hangouts
                                       where a.Id == h.Id
                                       select a).SingleOrDefault();
                    if (tempHangout != null)
                    {
                        var allAtendees = (from u in AspNetUsers_Hangout_db.AspNetUsers_Hangouts
                                           where u.HangoutId == tempHangout.Id && u.IsRSVPd == true
                                           select u);
                        var hostAverageRating = (from u in OrganizerRatings_db.OrganizerRatings
                                                 where u.OrganizerId == tempHangout.UserCreator
                                                 select u);
                        double sum = 0;

                        foreach(var r in hostAverageRating)
                        {
                            sum += (double) r.Rating;
                        }

                        // Use aggregate in future...it wasnt working before.
                        // test commit

                        var average = sum / hostAverageRating.Count();

                        var hangout = new HangoutViewModel
                        {
                            Date = tempHangout.Date,
                            Description = tempHangout.Description,
                            Id = tempHangout.Id,
                            Name = tempHangout.Name,
                            OpenSpots = tempHangout.PartySize - allAtendees.Count(),
                            HostName = GetUserName(tempHangout.UserCreator),
                            MaleOpenSpots = (tempHangout.PartySize/2) - tempHangout.MaleAttendingCount,
                            FemaleOpenSpots = (tempHangout.PartySize / 2) - tempHangout.FemaleAttendingCount,
                            GenderRatio = tempHangout.GenderRatio,
                            HostAverageRating = Math.Round(average, 2),
                            StartTime = tempHangout.StartTime,
                            EndTime = tempHangout.EndTime
                        };

                        foreach(var a in allAtendees)
                        {
                            var user = (from y in AspNetUsers_db.AspNetUsers
                                                where y.Id == a.AspNetUsers
                                                select y).SingleOrDefault();
                            hangout.AttendingList.Add(new UserViewModel{
                                username = user.UserName
                            });
                        }

                        returnList.Add(hangout);
                    }
                }
            }
            

            return returnList;
        }

        public string GetUserName(string userId)
        {
            return (from a in AspNetUsers_db.AspNetUsers
                    where a.Id == userId
                    select a).SingleOrDefault().UserName;
        }

        public List<HangoutViewModel> ListPastHangouts(string userId)
        {
            var allHangouts = from m in AspNetUsers_Hangout_db.AspNetUsers_Hangouts
                                    where m.AspNetUsers == userId 
                                    select m;
            var allPastHangouts = from m in Hangout_db.Hangouts
                             where m.Date < DateTime.Now
                             select m;

            var returnList = new List<HangoutViewModel>();
            foreach (var h in allPastHangouts.ToList())
            {
                var allAtendees = (from u in AspNetUsers_Hangout_db.AspNetUsers_Hangouts
                                   where u.HangoutId == h.Id && u.IsRSVPd == true
                                   select u);
                var hangoutAverageRating = (from u in HangoutRatings_db.HangoutRatings
                                         where u.HangoutId == h.Id
                                         select u);
                double sum = 0;

                foreach (var r in hangoutAverageRating)
                {
                    sum += (double)r.Rating;
                }

                // Use aggregate in future...it wasnt working before.

                var average = sum / hangoutAverageRating.Count();
                bool isRsvp = false;
                var isRsvpUserHangout = (from u in AspNetUsers_Hangout_db.AspNetUsers_Hangouts
                              where u.AspNetUsers == userId && u.HangoutId == h.Id && u.IsRSVPd == true
                              select u).SingleOrDefault();
                if(isRsvpUserHangout != null)
                {
                    isRsvp = isRsvpUserHangout.IsRSVPd;
                }
                var hangout = new HangoutViewModel
                {
                    Date = h.Date,
                    Description = h.Description,
                    Id = h.Id,
                    IsHost = h.UserCreator.Equals(userId) ? true : false,
                    IsRsvp = isRsvp,
                    Name = h.Name,
                    HostName = GetUserName(h.UserCreator),
                    HangoutAverageRating = Math.Round(average, 2),
                    StartTime = h.StartTime,
                    EndTime = h.EndTime
                    
                };

                foreach (var a in allAtendees)
                {
                    var user = (from y in AspNetUsers_db.AspNetUsers
                                where y.Id == a.AspNetUsers
                                select y).SingleOrDefault();
                    hangout.AttendingList.Add(new UserViewModel
                    {
                        username = user.UserName
                    });
                }

                returnList.Add(hangout);
                
            }
            

            return returnList;
        }

        public void JoinHangout(string userId, int hangoutId)
        {

            var Hangout = (from a in Hangout_db.Hangouts
                           where a.Id == hangoutId
                           select a).SingleOrDefault();
            var UserAccount = (from a in AspNetUsers_db.AspNetUsers
                               where a.Id == userId
                               select a).SingleOrDefault();
            if ((Hangout.AttendeeCount < Hangout.PartySize && Hangout.GenderRatio == false) || 
                (Hangout.AttendeeCount < Hangout.PartySize && Hangout.GenderRatio == true &&
                (UserAccount.Gender == 0 && Hangout.MaleAttendingCount < Hangout.PartySize / 2) || (UserAccount.Gender == 1 && Hangout.FemaleAttendingCount < Hangout.PartySize / 2)))
            {
                //if (AspNetUsers_Hangout_db.AspNetUsers_Hangouts.Any())
                //    {
                var UserHangout = (from a in AspNetUsers_Hangout_db.AspNetUsers_Hangouts
                                   where a.HangoutId == hangoutId && a.AspNetUsers == userId
                                   select a).SingleOrDefault();

                if (UserHangout == null)
                {
                    UserHangout = new AspNetUsers_Hangout
                    {
                        AspNetUsers = userId,
                        HangoutId = hangoutId,
                        IsRSVPd = true,
                        IsWaitlist = false
                    };

                    AspNetUsers_Hangout_db.AspNetUsers_Hangouts.InsertOnSubmit(UserHangout);
                    // does not ever submit need to submit changes.
                }
                else
                {
                    UserHangout.IsRSVPd = true;
                }
                //}

                try
                {
                    if (UserAccount.Gender == 0)
                    {
                        Hangout.MaleAttendingCount++;
                    }
                    else
                    {
                        Hangout.FemaleAttendingCount++;
                    }
                    Hangout.AttendeeCount++;

                    ActivityLog actLog = new ActivityLog
                    {
                        AspNetUserId = UserAccount.Id,
                        HangoutId = Hangout.Id,
                        TimeStamp = DateTimeOffset.Now.DateTime.ToLocalTime(),
                        ActivityType = (int)ActivityType.Join
                    };

                    ActivityLog_db.ActivityLogs.InsertOnSubmit(actLog);

                    Hangout_db.SubmitChanges();
                    AspNetUsers_Hangout_db.SubmitChanges();
                    ActivityLog_db.SubmitChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    // Make some adjustments. 
                    // ... 
                    // Try again.
                    Hangout_db.SubmitChanges();
                    AspNetUsers_Hangout_db.SubmitChanges();
                    ActivityLog_db.SubmitChanges();
                }
            }
            else 
            {
                // Hangout too full/Hangout has too many boys/girls. Cant join.
            }


        }

        public void WaitlistHangout(string userId, int hangoutId)
        {
            if (AspNetUsers_Hangout_db.AspNetUsers_Hangouts.Any())
            {
                var UserHangout = (from a in AspNetUsers_Hangout_db.AspNetUsers_Hangouts
                                   where a.HangoutId == hangoutId && a.AspNetUsers == userId
                                   select a).SingleOrDefault();

                if (UserHangout == null)
                {
                    UserHangout = new AspNetUsers_Hangout
                    {
                        AspNetUsers = userId,
                        HangoutId = hangoutId,
                        IsRSVPd = false,
                        IsWaitlist = true
                    };

                    AspNetUsers_Hangout_db.AspNetUsers_Hangouts.InsertOnSubmit(UserHangout);
                }
                else
                {
                        UserHangout.IsWaitlist = true;
                        UserHangout.IsRSVPd = false;
              
                }

            }
            else
            {

                var UserHangout = new AspNetUsers_Hangout
                {
                    AspNetUsers = userId,
                    HangoutId = hangoutId,
                    IsRSVPd = false,
                    IsWaitlist = true
                };

                AspNetUsers_Hangout_db.AspNetUsers_Hangouts.InsertOnSubmit(UserHangout);
            }

            try
            {
                AspNetUsers_Hangout_db.SubmitChanges();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                // Make some adjustments. 
                // ... 
                // Try again.
                AspNetUsers_Hangout_db.SubmitChanges();
            }    

        }

        public void LeaveHangout(string userId, int hangoutId)
        {
            if (AspNetUsers_Hangout_db.AspNetUsers_Hangouts.Any())
            {
                var UserHangout = (from a in AspNetUsers_Hangout_db.AspNetUsers_Hangouts
                                   where a.HangoutId == hangoutId && a.AspNetUsers == userId
                                   select a).SingleOrDefault();

                var UserAccount = (from a in AspNetUsers_db.AspNetUsers
                                   where a.Id == userId
                                   select a).SingleOrDefault();
                var Hangout = (from a in Hangout_db.Hangouts
                               where a.Id == hangoutId
                               select a).SingleOrDefault();
                if (UserAccount.Gender == 0)
                {
                    Hangout.MaleAttendingCount--;
                }
                else
                {
                    Hangout.FemaleAttendingCount--;
                }
                Hangout.AttendeeCount--;
                UserHangout.IsRSVPd = false;
            }

            try
            {
                Hangout_db.SubmitChanges();
                AspNetUsers_Hangout_db.SubmitChanges();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                // Make some adjustments. 
                // ... 
                // Try again.
                AspNetUsers_Hangout_db.SubmitChanges();
            }  
        }

        public ShowNoShowHangoutViewModel GetShowNoShow(int hangoutId)
        {
            var showNoShowHangoutViewModel = new ShowNoShowHangoutViewModel();
            var userHangout = (from a in AspNetUsers_Hangout_db.AspNetUsers_Hangouts
                               where a.HangoutId == hangoutId
                               select a);
            showNoShowHangoutViewModel.Id = hangoutId;

            foreach (var u in userHangout)
            {
                showNoShowHangoutViewModel.UserList.Add(new ShowOrNoShowUser
                {
                    AttendeeId = u.AspNetUsers,
                    Username = GetUserName(u.AspNetUsers),
                    Showed = u.Showed
                });
            }

            return showNoShowHangoutViewModel;

        }

        public void ShoworNoShowSubmit(ShowNoShowHangoutViewModel model)
        {
            foreach (var m in model.UserList)
            {
                var userHangout = (from a in AspNetUsers_Hangout_db.AspNetUsers_Hangouts
                                   where a.HangoutId == model.Id && a.AspNetUsers == m.AttendeeId
                                   select a).Single();
                userHangout.Showed = m.Showed;

                // Can I just make one submit call?
                try
                {
                    AspNetUsers_Hangout_db.SubmitChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    // Provide for exceptions.
                }
            }

        }

        public void RateOrganizerAndHangout(RateOrganizerHangoutViewModel model)
        {
            OrganizerRating orgRating = new OrganizerRating
            {
                OrganizerId = model.UserId,
                AttendeeId = model.AttendeeId,
                Rating = model.OrganizerRating
            };

            HangoutRating hangoutRating = new HangoutRating
            {
                AttendeeId = model.AttendeeId,
                HangoutId = model.Id,
                Rating = model.HangoutRating
            };

            OrganizerRatings_db.OrganizerRatings.InsertOnSubmit(orgRating);
            HangoutRatings_db.HangoutRatings.InsertOnSubmit(hangoutRating);


            try
            {
                OrganizerRatings_db.SubmitChanges();
                HangoutRatings_db.SubmitChanges();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                // Make some adjustments. 
                // ... 
                // Try again.
                OrganizerRatings_db.SubmitChanges();
                HangoutRatings_db.SubmitChanges();

            }
        }

        public void Create(CreateHangoutViewModel model)
        {
            Hangout hang = new Hangout
            {
                Date = DateTime.Parse(model.Date),
                Description = model.Description,
                Name = model.Name,
                UserCreator = model.UserId,
                Location = model.Location,
                Address = model.Address,
                PartySize = model.PartySize,
                ContactInfo = model.ContactInfo,
                GenderRatio = model.GenderRatio,
                StartTime = DateTime.Parse(model.StartTime),
                EndTime = DateTime.Parse(model.EndTime)
                
            };

            //Need to grab user in controller
            Hangout_db.Hangouts.InsertOnSubmit(hang);

            

            try
            {
                Hangout_db.SubmitChanges();
                // TODO: Add log entry for create event.
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                // Make some adjustments. 
                // ... 
                // Try again.
                Hangout_db.SubmitChanges();
            }

            ActivityLog actLog = new ActivityLog
            {
                AspNetUserId = model.UserId,
                HangoutId = hang.Id,
                TimeStamp = DateTimeOffset.Now.DateTime.ToLocalTime(),
                ActivityType = (int) ActivityType.Create
            };

            ActivityLog_db.ActivityLogs.InsertOnSubmit(actLog);

            try
            {
                ActivityLog_db.SubmitChanges();
                // TODO: Add log entry for create event.
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                // Make some adjustments. 
                // ... 
                // Try again.
                ActivityLog_db.SubmitChanges();
            }

        }

        public void Delete(int id)
        {
            Hangout_db.Hangouts.DeleteOnSubmit(Hangout_db.Hangouts.Where(c => c.Id == id).Select(c => c).Single());

            try
            {
                Hangout_db.SubmitChanges();
            }            
            catch (Exception e)
            {
                Console.WriteLine(e);
                // Make some adjustments. 
                // ... 
                // Try again.
                Hangout_db.SubmitChanges();
            }
         }

        public Hangout GetHangoutById(int id)
        {
            var hangout = (from a in Hangout_db.Hangouts
                         where a.Id == id
                         select a).Single();
            return hangout;
        }

        public void Update(CreateHangoutViewModel model)
        {
            var hangout = (from a in Hangout_db.Hangouts
                           where a.Id == model.Id
                           select a).Single();
            hangout.Date = DateTime.Parse(model.Date);
            hangout.Description = model.Description;
            hangout.Name = model.Name;
            hangout.Address = model.Address;
            hangout.ContactInfo = model.ContactInfo;
            hangout.Location = model.Location;
            hangout.PartySize = model.PartySize;
            hangout.GenderRatio = model.GenderRatio;
            DateTime startTimeOut, endTimeOut;
            if (DateTime.TryParse(model.StartTime, out startTimeOut))
            {
                hangout.StartTime = startTimeOut;
            }
            if (DateTime.TryParse(model.EndTime, out endTimeOut))
            {
                hangout.EndTime = endTimeOut;
            }

            try
            {
                Hangout_db.SubmitChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                // Provide for exceptions.
            }

        }

        public int GetStrikeCount(string userId)
        {
            return (from a in AspNetUsers_Hangout_db.AspNetUsers_Hangouts
                    where a.AspNetUsers == userId && a.Showed == false
                    select a).Count();
        }

        public string GetUserGender(string userId)
        {
            return (from a in AspNetUsers_db.AspNetUsers
                    where a.Id == userId
                    select a).SingleOrDefault().Gender == 0 ? "male" : "female";
        }

        public double GetUserRating(string userId)
        {
            var hostAverageRating = (from u in OrganizerRatings_db.OrganizerRatings
                                     where u.OrganizerId == userId
                                     select u);
            double sum = 0;

            foreach (var r in hostAverageRating)
            {
                sum += (double)r.Rating;
            }

            // Use aggregate in future...it wasnt working before.

            var average = sum / hostAverageRating.Count();

            return Math.Round(average, 2);
        }

        public string GetUserProfilePicture(string userId)
        {
            var dir = HttpContext.Current.Server.MapPath("~/Images");
            return Path.Combine(dir, "am_eskimo3" + ".jpg");
        }
    }
}
﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
            //var myHangouts = from m in Hangout_db.Hangouts
            //                 where m.UserCreator == userId && new DateTime(m.Date.Value.Year, m.Date.Value.Month, m.Date.Value.Day, m.StartTime.Value.Hour, m.StartTime.Value.Minute, m.StartTime.Value.Second)  > DateTime.UtcNow.AddHours(-7)
            //                        && m.IsCancelled == false   
            //                 select m;

            var myHangouts = from m in Hangout_db.Hangouts
                             where m.UserCreator == userId && new DateTime(m.Date.Value.Year, m.Date.Value.Month, m.Date.Value.Day, m.StartTime.Value.Hour, m.StartTime.Value.Minute, m.StartTime.Value.Second) > DateTime.UtcNow.AddHours(-7).AddDays(-7)
                                    && m.IsCancelled == false
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
                    HostUser = GetUser(h.UserCreator),
                    IsHost = true,
                    IsRsvp = false,
                    HostAverageRating = Math.Round(average, 2),
                    Location = h.Location,
                    ImageContent = h.ImageContent == null ? new byte[8] : h.ImageContent.ToArray(),
                    ImageMimeType = h.ImageMimeType == null ? "none" : h.ImageMimeType
                };

                foreach (var a in allAtendees)
                {
                    var user = (from y in AspNetUsers_db.AspNetUsers
                                where y.Id == a.AspNetUsers
                                select y).SingleOrDefault();
                    hangout.AttendingList.Add(GetUser(user.Id));
                }
                returnList.Add(hangout);
            }
            // Map to returnList
            if(hangoutsAttending.Any())
            {
                foreach(var h in hangoutsAttending)
                {
                    //var tempHangout = (from a in Hangout_db.Hangouts
                    //                   where a.Id == h.HangoutId && new DateTime(a.Date.Value.Year, a.Date.Value.Month, a.Date.Value.Day, a.StartTime.Value.Hour, a.StartTime.Value.Minute, a.StartTime.Value.Second) > DateTime.UtcNow.AddHours(-7)
                    //                   && a.IsCancelled == false
                    //               select a).SingleOrDefault();
                    var tempHangout = (from a in Hangout_db.Hangouts
                                       where a.Id == h.HangoutId
                                       && a.IsCancelled == false
                                       select a).SingleOrDefault();


                    
                    
                    if (tempHangout != null)
                    {
                        var rated = (from a in OrganizerRatings_db.OrganizerRatings
                                     where a.HangoutId == h.HangoutId && a.OrganizerId == tempHangout.UserCreator && a.AttendeeId == userId
                                    select a).SingleOrDefault();
                        if(rated == null)
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
                                StartTime = tempHangout.StartTime,
                                EndTime = tempHangout.EndTime,
                                Description = tempHangout.Description,
                                Id = tempHangout.Id,
                                Name = tempHangout.Name,
                                OpenSpots = tempHangout.PartySize - allAtendees.Count(),
                                HostName = GetUserName(tempHangout.UserCreator),
                                HostUser = GetUser(tempHangout.UserCreator),
                                HostAverageRating = Math.Round(average, 2),
                                IsRsvp = true,
                                IsHost = false,
                                Location = tempHangout.Location,
                                ImageContent = tempHangout.ImageContent == null ? new byte[8] : tempHangout.ImageContent.ToArray(),
                                ImageMimeType = tempHangout.ImageMimeType == null ? "none" : tempHangout.ImageMimeType
                            };

                            foreach (var a in allAtendees)
                            {
                                var user = (from y in AspNetUsers_db.AspNetUsers
                                            where y.Id == a.AspNetUsers
                                            select y).SingleOrDefault();
                                hangout.AttendingList.Add(GetUser(user.Id));
                            }
                            returnList.Add(hangout);
                        }                      
                    }
                }
             }

            // Sorts the list ascending
            var returnHangouts = from m in returnList
                              orderby new DateTime(m.Date.Value.Year, m.Date.Value.Month, m.Date.Value.Day, m.StartTime.Value.Hour, m.StartTime.Value.Minute, m.StartTime.Value.Second) ascending
                              select m;
            return returnHangouts.ToList();
        }

        public List<ActivityViewModel> ListActivityLog()
        {
            var returnList = new List<ActivityViewModel>();
            var activities = (from m in ActivityLog_db.ActivityLogs
                             orderby m.TimeStamp descending
                                    select m).Take(30);
            foreach (var a in activities.ToList())
            {
                var activity = new ActivityViewModel
                {
                    activityType = a.ActivityType,
                    hangoutName = GetHangoutById(a.HangoutId).Name,
                    timeStamp = a.TimeStamp.ToLocalTime(),
                    name = GetName(a.AspNetUserId),
                    userId = a.AspNetUserId,
                    User = GetUser(a.AspNetUserId)
                };
                returnList.Add(activity);
            }
            return returnList;
        }


        public List<HangoutViewModel> ListUpcomingHangouts(string userId)
        {
            var allHangouts = from m in Hangout_db.Hangouts
                             where new DateTime(m.Date.Value.Year, m.Date.Value.Month, m.Date.Value.Day, m.StartTime.Value.Hour, m.StartTime.Value.Minute, m.StartTime.Value.Second) > DateTime.UtcNow.AddHours(-7)
                             && m.IsCancelled == false
                              orderby new DateTime(m.Date.Value.Year, m.Date.Value.Month, m.Date.Value.Day, m.StartTime.Value.Hour, m.StartTime.Value.Minute, m.StartTime.Value.Second) ascending
                              select m;

            var returnList = new List<HangoutViewModel>();
            foreach(var tempHangout in allHangouts)
            {
                if (tempHangout != null)
                {
                    var hangout = HangoutToHangoutViewModel(userId, tempHangout);
                    returnList.Add(hangout);
                }
            }
            

            return returnList;
        }

        public HangoutViewModel GetHangoutViewModelById(string userId, int id)
        {
            var hangout = GetHangoutById(id);
            if (hangout != null)
            {
                var hangoutViewModel = HangoutToHangoutViewModel(userId, hangout);
                return hangoutViewModel;
            }
            return new HangoutViewModel();
        }

        private HangoutViewModel HangoutToHangoutViewModel(string userId, Hangout hangout)
        {
            var allAtendees = (from u in AspNetUsers_Hangout_db.AspNetUsers_Hangouts
                where u.HangoutId == hangout.Id && u.IsRSVPd == true
                select u);

            var hostAverageRating = (from u in OrganizerRatings_db.OrganizerRatings
                where u.OrganizerId == hangout.UserCreator
                select u);
            double sum = 0;

            foreach (var r in hostAverageRating)
            {
                sum += (double) r.Rating;
            }

            var hostAvg = sum/hostAverageRating.Count();
            bool isRsvp = false;
            var isRsvpUserHangout = (from u in AspNetUsers_Hangout_db.AspNetUsers_Hangouts
                where u.AspNetUsers == userId && u.HangoutId == hangout.Id && u.IsRSVPd == true
                select u).SingleOrDefault();
            if (isRsvpUserHangout != null)
            {
                isRsvp = isRsvpUserHangout.IsRSVPd;
            }

            var hangoutViewModel = new HangoutViewModel
            {
                Date = hangout.Date,
                Description = hangout.Description,
                Id = hangout.Id,
                Name = hangout.Name,
                Address = hangout.Address,
                ContactInfo = hangout.ContactInfo,
                OpenSpots = hangout.PartySize - allAtendees.Count(),
                HostName = GetUserName(hangout.UserCreator),
                MaleOpenSpots = (hangout.PartySize/2) - hangout.MaleAttendingCount,
                FemaleOpenSpots = (hangout.PartySize/2) - hangout.FemaleAttendingCount,
                GenderRatio = hangout.GenderRatio,
                HostAverageRating = Math.Round(hostAvg, 2),
                StartTime = hangout.StartTime,
                EndTime = hangout.EndTime,
                IsHost = hangout.UserCreator.Equals(userId) ? true : false,
                HostUser = GetUser(hangout.UserCreator),
                IsRsvp = isRsvp,
                Location = hangout.Location,
                ImageContent = hangout.ImageContent == null ? new byte[8] : hangout.ImageContent.ToArray(),
                ImageMimeType = hangout.ImageMimeType == null ? "none" : hangout.ImageMimeType
            };

            foreach (var a in allAtendees)
            {
                var user = (from y in AspNetUsers_db.AspNetUsers
                    where y.Id == a.AspNetUsers
                    select y).SingleOrDefault();
                hangoutViewModel.AttendingList.Add(GetUser(user.Id));
            }
            return hangoutViewModel;
        }

        public string GetUserName(string userId)
        {
            return (from a in AspNetUsers_db.AspNetUsers
                    where a.Id == userId
                    select a).SingleOrDefault().UserName;
        }

        public string GetName(string userId)
        {
            return (from a in AspNetUsers_db.AspNetUsers
                    where a.Id == userId
                    select a).SingleOrDefault().Name;
        }

        public List<HangoutViewModel> ListPastHangouts(string userId)
        {
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
                        Name = user.UserName
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
                        IsWaitlist = false,
                        Showed = true
                    };

                    AspNetUsers_Hangout_db.AspNetUsers_Hangouts.InsertOnSubmit(UserHangout);
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
                        TimeStamp = DateTime.UtcNow,
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
                Rating = model.OrganizerRating,
                HangoutId = model.Id
            };

            //HangoutRating hangoutRating = new HangoutRating
            //{
            //    AttendeeId = model.AttendeeId,
            //    HangoutId = model.Id,
            //    Rating = model.HangoutRating
            //};

            OrganizerRatings_db.OrganizerRatings.InsertOnSubmit(orgRating);
            //HangoutRatings_db.HangoutRatings.InsertOnSubmit(hangoutRating);


            try
            {
                OrganizerRatings_db.SubmitChanges();
                //HangoutRatings_db.SubmitChanges();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                // Make some adjustments. 
                // ... 
                // Try again.
                OrganizerRatings_db.SubmitChanges();
                //HangoutRatings_db.SubmitChanges();

            }
        }

        public void Create(CreateHangoutViewModel model)
        {
            
            Hangout hang = new Hangout
            {
                Date = model.Date,
                Description = model.Description,
                Name = model.Name,
                UserCreator = model.UserId,
                Location = model.Location,
                Address = model.Address,
                PartySize = model.PartySize,
                ContactInfo = model.ContactInfo,
                GenderRatio = model.GenderRatio,
                StartTime = model.StartTime,
                EndTime = model.EndTime,
                AttendeeCount = 1,
                IsCancelled = false,
                ImageContent = new byte[8],
                ImageMimeType = "0"
            };
            if (model.ImageUpload != null && model.ImageUpload.ContentLength > 0)
            {
                using (var binaryReader = new BinaryReader(model.ImageUpload.InputStream))
                {
                    hang.ImageContent = binaryReader.ReadBytes(model.ImageUpload.ContentLength);
                }
                hang.ImageMimeType = model.ImageUpload.ContentType;
            }
            

            string gend = GetUserGender(model.UserId);
            if(gend.Equals("male"))
            {
                hang.MaleAttendingCount++;
            }
            else if(gend.Equals("female"))
            {
                hang.FemaleAttendingCount++;
            }

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
                TimeStamp = DateTime.UtcNow,
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

        public void Cancel(int id)
        {
            var hangout = GetHangoutById(id);
            hangout.IsCancelled = true;

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

        public HangoutViewModel GetRateHangoutById(int Id)
        {
            var h = GetHangoutById(Id);
            var allAtendees = (from u in AspNetUsers_Hangout_db.AspNetUsers_Hangouts
                               where u.HangoutId == h.Id && u.IsRSVPd == true
                               select u);
            var returnThis =  new HangoutViewModel
            {
                Date = h.Date,
                StartTime = h.StartTime,
                EndTime = h.EndTime,
                Description = h.Description,
                Id = h.Id,
                Name = h.Name,
                OpenSpots = h.PartySize - allAtendees.Count(),
                HostName = GetUserName(h.UserCreator),
                HostUser = GetUser(h.UserCreator),
                IsHost = true,
                IsRsvp = false,
                Location = h.Location,
                ImageContent = h.ImageContent == null ? new byte[8] : h.ImageContent.ToArray(),
                ImageMimeType = h.ImageMimeType == null ? "none" : h.ImageMimeType
            };

            foreach (var a in allAtendees)
            {
                var user = (from y in AspNetUsers_db.AspNetUsers
                            where y.Id == a.AspNetUsers
                            select y).SingleOrDefault();
                returnThis.AttendingList.Add(GetUser(user.Id));
            }
            return returnThis;
        }

        public void Update(CreateHangoutViewModel model)
        {
            var hangout = (from a in Hangout_db.Hangouts
                           where a.Id == model.Id
                           select a).Single();
            hangout.Date = model.Date;
            hangout.Description = model.Description;
            hangout.Name = model.Name;
            hangout.Address = model.Address;
            hangout.ContactInfo = model.ContactInfo;
            hangout.Location = model.Location;
            hangout.PartySize = model.PartySize;
            hangout.GenderRatio = model.GenderRatio;
            
            if (model.ImageUpload != null && model.ImageUpload.ContentLength > 0)
            {
                using (var binaryReader = new BinaryReader(model.ImageUpload.InputStream))
                {
                    hangout.ImageContent = binaryReader.ReadBytes(model.ImageUpload.ContentLength);
                }
                hangout.ImageMimeType = model.ImageUpload.ContentType;
            }
            
            DateTime startTimeOut, endTimeOut;
            if (model.StartTime.HasValue)
            {
                hangout.StartTime = model.StartTime;
            }
            if (model.EndTime.HasValue)
            {
                hangout.EndTime = model.EndTime;
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

        public void UpdateName(string userId, string name)
        {
            var user = (from a in AspNetUsers_db.AspNetUsers
                           where a.Id == userId
                           select a).Single();
            user.Name = name;

            try
            {
                AspNetUsers_db.SubmitChanges();
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
            if(sum > 0)
            {
                var average = sum / hostAverageRating.Count();
                return Math.Round(average, 2);
            }
            else
            {
                return 0;
            }

        }

        public string GetUserProfilePicture(string userId)
        {
            var dir = HttpContext.Current.Server.MapPath("~/Images");
            return Path.Combine(dir, "am_eskimo3" + ".jpg");
        }

        public UserViewModel GetUser(string userId)
        {
            var user = (from a in AspNetUsers_db.AspNetUsers
                        where a.Id == userId
                        select a).SingleOrDefault();
            return new UserViewModel
            {
                UserId = user.Id,
                Gender = GetUserGender(userId),
                Name = user.Name,
                UserRating = GetUserRating(userId),
                NoShows = GetStrikeCount(userId),
                ImageContent = user.ImageContent == null ? new byte[8] : user.ImageContent.ToArray(),
                ImageMimeType = user.ImageMimeType == null ? "none" : user.ImageMimeType
            };
            // Put hangouts attended and hangouts hosted.
        }

        public void UploadPhoto(HttpPostedFileBase image, string userId)
        {
            var user = (from a in AspNetUsers_db.AspNetUsers
                        where a.Id == userId
                        select a).SingleOrDefault();
            using (var binaryReader = new BinaryReader(image.InputStream))
            {
                user.ImageContent = binaryReader.ReadBytes(image.ContentLength);
            }
            user.ImageMimeType = image.ContentType;

            try
            {
                AspNetUsers_db.SubmitChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                // Provide for exceptions.
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using Newtonsoft;
using System.Net;
using System.Net.Mail;
using System.Web.Mvc;
using System.Web.Services;
using Microsoft.AspNet.Identity;
using MyModel.WashTime;
using VaskerietOMA.DataAccess;
using VaskerietOMA.DatabaseFunction;
using VaskerietOMA.Models;
using VaskerietOMA.ViewModel;

namespace VaskerietOMA.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext applicationDb = new ApplicationDbContext();
        private WashContext db = new WashContext();
        private DatabaseFiller filler =new DatabaseFiller();

       public ActionResult Index()
        {
            List<WashTime> times = db.WashTimes.Where(x => x.Time.Year == DateTime.Today.Year).ToList();
            List<WashTime> thisWeek =
                times.Where(
                    time =>
                        HelperFunctions.GetIso8601WeekOfYear(time.Time) ==
                        HelperFunctions.GetIso8601WeekOfYear(DateTime.Today)
                    ).OrderBy(f => f.Time.Date).ThenBy(f => f.Time.Hour).ThenBy(f => f.Machine
                    ).GroupBy(
                        f => new { f.Time.Date, f.Time.Hour, f.Machine }
                    ).Select(
                        group => group.First()
                    ).ToList();


            if (thisWeek.Count<(126*2))
            {
                FillWeek();
                times = db.WashTimes.Where(x => x.Time.Year == DateTime.Today.Year).ToList();
                thisWeek =
                                times.Where(
                                    time =>
                                        HelperFunctions.GetIso8601WeekOfYear(time.Time) ==
                                        HelperFunctions.GetIso8601WeekOfYear(DateTime.Today)
                                    ).OrderBy(f => f.Time.Date).ThenBy(f => f.Time.Hour).ThenBy(f => f.Machine
                                    ).GroupBy(
                                        f => new { f.Time.Date, f.Time.Hour, f.Machine }
                                    ).Select(
                                        group => group.First()
                                    ).ToList();
            }

            TimeTableViewModel weekTableViewModel = new TimeTableViewModel(thisWeek) {User = GetBookingViewModel()};

            if (Request.IsAjaxRequest())
            {
                return (PartialView("Index", weekTableViewModel));
            }

            return View(weekTableViewModel);
        }

        

        private void FillWeek()
        {
            filler.FillDatabaseWeek("Left");
            filler.FillDatabaseWeek("Right");
            
        }

        public ActionResult Bar()
        {
            BarBookingVmList vm = new BarBookingVmList
            {
                BarBookings = db.BarBookings.ToList().Select(c => new BarBookingVM(c)).ToList()
            };
            return View(vm);
        }

        public ActionResult BarBookingAdministration()
        {
            BarBookingVmList vm = new BarBookingVmList
            {
                BarBookings = db.BarBookings.ToList().Select(c => new BarBookingVM(c)).ToList()
            };
            return View(vm);
        }

        public ActionResult About()
        {
            if (Request.IsAjaxRequest())
            {
                return (PartialView("About"));
            }
            return View();
        }

        public ActionResult Contact()
        {
            if (Request.IsAjaxRequest())
            {
                return (PartialView("Contact"));
            }
            return View();
        }

        [HttpPost]
        public bool EventApproved(BarBookingVM barBooking)
        {
            if (ModelState.IsValid)
            {
                var booking = db.BarBookings.Find(barBooking.Id);
                if (booking == null)
                {
                    return false;
                }
                booking.Status = (int) BarBookingStatus.BookingApproved;
                db.Entry(booking).State = EntityState.Modified;
                try
                {
                    db.SaveChanges();
                }
                catch (Exception)
                {
                    return false;
                }
                //SmtpClient client = new SmtpClient
                //{
                //    Port = 587,
                //    DeliveryMethod = SmtpDeliveryMethod.Network,
                //    UseDefaultCredentials = false,
                //    Credentials = new NetworkCredential(ConfigurationManager.AppSettings["sendMailUser"], ConfigurationManager.AppSettings["sendMailPassword"]),
                //    Host = "smtp.unoeuro.com"
                //};

                //var body = "<p>Din betaling til booking af baren: d. {0} er registeret </p>";
                //var message = new MailMessage();
                //message.From = (new MailAddress("administrator@vaskerietoma.dk", "Vaskeriet OMA")); //replace with valid value
                //message.To.Add(new MailAddress(booking.Email));
                //message.Subject = "Booking ad baren til " + barBooking.Name;
                //message.Body = string.Format(body, barBooking.StartTime.ToString(CultureInfo.CurrentCulture));
                //message.IsBodyHtml = true;

                //client.Send(message);

                return true;
            }
            return false;
        }


        [HttpPost]
        public bool EventPaid(BarBookingVM barBooking)
        {
            if (ModelState.IsValid)
            {
                var booking = db.BarBookings.Find(barBooking.Id);
                if (booking == null)
                {
                    return false;
                }
                booking.IsPaid = true;
                db.Entry(booking).State = EntityState.Modified;
                try
                {
                    db.SaveChanges();
                }
                catch (Exception)
                {
                    return false;
                }
                SmtpClient client = new SmtpClient
                {
                    Port = 587,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(ConfigurationManager.AppSettings["sendMailUser"], ConfigurationManager.AppSettings["sendMailPassword"]),
                    Host = "smtp.unoeuro.com"
                };

                var body = "<p>Din betaling til booking af baren: d. {0} er registeret </p>";
                var message = new MailMessage();
                message.From = (new MailAddress("administrator@vaskerietoma.dk", "Vaskeriet OMA")); //replace with valid value
                message.To.Add(new MailAddress(booking.Email));
                message.Subject = "Booking ad baren til " + barBooking.Name;
                message.Body = string.Format(body, barBooking.StartTime.ToString(CultureInfo.CurrentCulture));
                message.IsBodyHtml = true;

                client.Send(message);

                return true;
            }
            return false;
        }


        [HttpPost]
        public string BookBar(BarBookingVM barBooking)
        {
            if (ModelState.IsValid)
            {
                if (barBooking.EndTime < barBooking.StartTime)
                {
                    barBooking.EndTime = barBooking.EndTime.AddDays(1);
                }
                BarBooking booking = barBooking.ToData();
                db.BarBookings.Add(booking);
                db.SaveChangesAsync();

                SmtpClient client = new SmtpClient
                {
                    Port = 587,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(ConfigurationManager.AppSettings["sendMailUser"], ConfigurationManager.AppSettings["sendMailPassword"]),
                    Host = "smtp.unoeuro.com"
                };

                var body = "<p>Email From: {0} ({1})</p>"
                        + "<p>Baren ønskes bookes d. {2} - {3} </p>" +
                        "<p>Besked:</p><p>{4}</p>";
                var message = new MailMessage();
                message.From = (new MailAddress("administrator@vaskerietoma.dk", "Vaskeriet OMA")); //replace with valid value
                message.To.Add(new MailAddress("sthranehansen@gmail.com"));
                message.Subject = "Booking ad baren til " + barBooking.Name;
                message.Body = string.Format(body, barBooking.Organizer, barBooking.Email, barBooking.StartTime.ToString(CultureInfo.CurrentCulture), barBooking.EndTime.ToString(CultureInfo.CurrentCulture), barBooking.Message);
                message.IsBodyHtml = true;

                client.Send(message);


                return Newtonsoft.Json.JsonConvert.SerializeObject(new BarBookingVM(booking));
            }

            return Newtonsoft.Json.JsonConvert.SerializeObject(barBooking);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Contact(EmailFormModel model)
        {
            if (ModelState.IsValid)
            {
                SmtpClient client = new SmtpClient
                {
                    Port = 587,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(ConfigurationManager.AppSettings["sendMailUser"], ConfigurationManager.AppSettings["sendMailPassword"]),
                    Host = "smtp.unoeuro.com"
                };

                var body = "<p>Email From: {0} ({1})</p><p>Message:</p><p>{2}</p>";
                var message = new MailMessage();
                message.To.Add(new MailAddress("sthranehansen@gmail.com")); //replace with valid value
                message.Subject = "Fejlmeddelse af maskine";
                message.Body = string.Format(body, model.FromName, model.FromEmail, model.Message);
                message.IsBodyHtml = true;
                if (model.Upload != null && model.Upload.ContentLength > 0)
                {
                    message.Attachments.Add(new Attachment(model.Upload.InputStream, Path.GetFileName(model.Upload.FileName)));
                }
                client.Send(message);
            }

            return RedirectToAction("Index", "Home");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ContactIdea(EmailFormModel model)
        {
            if (ModelState.IsValid)
            {
                SmtpClient client = new SmtpClient
                {
                    Port = 587,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(ConfigurationManager.AppSettings["sendMailUser"], ConfigurationManager.AppSettings["sendMailPassword"]),
                    Host = "smtp.unoeuro.com"
                };

                var body = "<p>Email From: {0} ({1})</p><p>Message:</p><p>{2}</p>";
                var message = new MailMessage();
                message.To.Add(new MailAddress("sthranehansen@gmail.com")); //replace with valid value
                message.Subject = "Ide til Vaskeriet OMA";
                message.Body = string.Format(body, model.FromName, model.FromEmail, model.Message);
                message.IsBodyHtml = true;
                if (model.Upload != null && model.Upload.ContentLength > 0)
                {
                    message.Attachments.Add(new Attachment(model.Upload.InputStream, Path.GetFileName(model.Upload.FileName)));
                }

                client.Send(message);
            }
            
            return RedirectToAction("Index", "Home");
        }


        [WebMethod]
        public JsonResult GetTopList()
        {
            var washtimes = db.WashTimes.Where(x => x.RoomNumber != 0).
                GroupBy(x => x.RoomNumber)
                .Select(g => new
                {
                    RoomNumber = g.Key,
                    count = g.Count()
                }).Take(10);

            TopListViewModel topListViewModel=new TopListViewModel();
            foreach (var time in washtimes)
            {
                topListViewModel.Entries.Add(new TopListEntry
                {
                    RoomNumber = time.RoomNumber,
                    Count = time.count
                });
            }


            return Json(topListViewModel, JsonRequestBehavior.AllowGet);
        }

        
        public TimeTableViewModel getTimeTable()
        {
            TimeTableViewModel currentWeekTimeTableViewModel =  new TimeTableViewModel();
            return currentWeekTimeTableViewModel;
        }

        private BookingViewModel GetBookingViewModel()
        {
            var user = applicationDb.Users.Find(System.Web.HttpContext.Current.User.Identity.GetUserId());
            var currentSessionUser = user != null ? new BookingViewModel(user) : new BookingViewModel();

            return currentSessionUser;

        }

        [WebMethod]
        public String GetTimeTableByDay(DateTime day)
        {
            var model = new TimeTableViewModel(day);
            return Newtonsoft.Json.JsonConvert.SerializeObject(model);

        }

        [WebMethod]
        public Boolean BookTime(WashTimeViewModel vm)
        {
            if (vm.Time > DateTime.Now)
            {
                WashTime currentbooking = db.WashTimes.Find(vm.ID);
                if (currentbooking == null)
                {
                    return false;
                }

                currentbooking.IsBooked = true;
                currentbooking.RoomNumber = vm.RoomNumber;
                db.Entry(currentbooking).State = EntityState.Modified;
                try
                {
                    db.SaveChangesAsync();
                    return true;

                }
                catch (Exception)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

        }

        [WebMethod]
        public Boolean RemoveUser(User delUser)
        {
            ApplicationUser user = applicationDb.Users.Find(delUser.Id);
            if (user == null)
            {
                return false;
            }
            user.UserName = String.Empty;
            user.Name = String.Empty;
            user.RoomNumber = 0;
            user.Email = String.Empty;

            db.Entry(user).State = EntityState.Modified;
            try
            {
                db.SaveChangesAsync();
                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        [WebMethod]
        public Boolean CancelEvent(BarBookingVM vm)
        {
            BarBooking currentbooking = db.BarBookings.Find(vm.Id);
            if (currentbooking == null)
            {
                return false;
            }
            db.BarBookings.Remove(currentbooking);
            try
            {
                db.SaveChanges();
                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        [WebMethod]
        public Boolean CancelBooking(WashTimeViewModel vm)
        {
            WashTime currentbooking = db.WashTimes.Find(vm.ID);
            if (currentbooking == null)
            {
                return false;
            }
            currentbooking.IsBooked = false;
            currentbooking.RoomNumber = 0;
            db.Entry(currentbooking).State = EntityState.Modified;
            try
            {
                db.SaveChanges();
                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
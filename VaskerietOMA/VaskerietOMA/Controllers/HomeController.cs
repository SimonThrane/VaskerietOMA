using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web.Mvc;
using System.Web.Services;
using MyModel.WashTime;
using VaskerietOMA.DataAccess;
using VaskerietOMA.DatabaseFunction;
using VaskerietOMA.ViewModel;

namespace VaskerietOMA.Controllers
{
    public class HomeController : Controller
    {
        private WashContext db = new WashContext();
        private DatabaseFiller filler =new DatabaseFiller();

       public ActionResult Index()
        {
            List<WashTime> times = db.WashTimes.Where(x => x.Time.Year == DateTime.Today.Year).ToList();
            //List<WashTime> thisWeek = times.Where(time => HelperFunctions.GetIso8601WeekOfYear(time.Time) == HelperFunctions.GetIso8601WeekOfYear(DateTime.Today)).ToList();
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
                thisWeek = times.Where(time => HelperFunctions.GetIso8601WeekOfYear(time.Time) == HelperFunctions.GetIso8601WeekOfYear(DateTime.Today)).ToList();
            }
            ViewBag.Weeknumber = HelperFunctions.GetIso8601WeekOfYear(DateTime.Now);

            TimeTableViewModel weekTableViewModel = new TimeTableViewModel(thisWeek);

            return View(weekTableViewModel);
        }

        private void FillWeek()
        {
            filler.FillDatabaseWeek("Left");
            filler.FillDatabaseWeek("Right");
            
        }

        public ActionResult Day()
        {
            List<WashTime> times = db.WashTimes.Where(x => x.Time.Day == DateTime.Today.Day).ToList();
            List<WashTime> thisWeek = times.Where(time =>time.Time.Date == DateTime.Today).ToList();
            if (thisWeek.Count < 16)
            {
                filler.FillDay(DateTime.Today, "Left");
                filler.FillDay(DateTime.Today, "Right");
            }

            if (Request.IsAjaxRequest())
            {
                return (PartialView(thisWeek.ToList()));
            }
            return View("Day", thisWeek.ToList());
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
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
        [ValidateAntiForgeryToken]
        public ActionResult Contact(string Name, string Machine, string Text)
        {
            SmtpClient client = new SmtpClient
            {
                Port = 587,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("administrator@vaskerietoma.dk", "23HejMed5"),
                Host = "smtp.unoeuro.com"
            };
            MailMessage mail = new MailMessage("administrator@vaskerietoma.dk", "sthranehansen@gmail.com")
            {
                Subject = "Fejlmeddelse af den " + Machine + " vaskemaskine",
                Body = $"{Text} \n Mvh \n {Name}"
            };
            client.Send(mail);

            return RedirectToAction("Index","Home");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ContactIdea(string Name, string Text)
        {
            SmtpClient client = new SmtpClient
            {
                Port = 587,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("administrator@vaskerietoma.dk", "23HejMed5"),
                Host = "smtp.unoeuro.com"
            };
            MailMessage mail = new MailMessage("administrator@vaskerietoma.dk", "sthranehansen@gmail.com")
            {
                Subject = "Forslag til forbedring af VaskerietOMa",
                Body = $"{Text} \n Mvh \n {Name}"
            };
            client.Send(mail);

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

        [WebMethod]
        public Boolean BookTime(WashTimeViewModel vm)
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
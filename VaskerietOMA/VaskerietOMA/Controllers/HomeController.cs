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
                   ).OrderBy(f => f.Time.Date).ThenBy(f => f.Time.Hour).ThenBy(f=>f.Machine
                   ).GroupBy(
                       f => new {f.Time.Date, f.Time.Hour, f.Machine}
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
            return View(thisWeek.ToList());
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

        //public ActionResult Day(DateTime day)
        //{
        //    List<WashTime> times = db.WashTimes.Where(x => x.Time.Day == day.Day).ToList();
        //    List<WashTime> thisWeek = times.Where(time => time.Time.Date == day.Date).ToList();
        //    if (thisWeek.Count < 16)
        //    {
        //        filler.FillDay(day,"Left");
        //        filler.FillDay(day, "Right");
        //    }

        //    if (Request.IsAjaxRequest())
        //    {
        //        return (PartialView(thisWeek.ToList()));
        //    }
        //    return View("Day", thisWeek.ToList());
        //}

       

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

        public ActionResult Booking(int id)
        {
            WashTime currentbooking = db.WashTimes.Find(id);
            if (currentbooking == null)
            {
                return HttpNotFound();
            }

            return PartialView("_Booking", currentbooking);
        }

        public bool isBooked(int ID)
        {
            WashTime currentbooking = db.WashTimes.Find(ID);
            if (currentbooking == null)
            {
                return false;
            }

            return currentbooking.IsBooked;
        }

        public int GetRoomNumber(int ID)
        {
            WashTime currentbooking = db.WashTimes.Find(ID);
            if (currentbooking == null)
            {
                return -1;
            }

            return currentbooking.RoomNumber;
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

        [HttpPost]
        public string CancelBooking(int ID)
        {
           
            WashTime currentbooking = db.WashTimes.Find(ID);
            if (currentbooking == null)
            {
                return "Det lykkedes desværre ikke at afbooke tiden";
            }
            string returnstring = "";
            currentbooking.IsBooked = false;
            currentbooking.RoomNumber = 0;
            db.Entry(currentbooking).State = EntityState.Modified;
            try
            {
                db.SaveChanges();
                returnstring = "Det lykkedes " + currentbooking.Machine + " er blevet afbooket " +
                               currentbooking.Time.ToShortTimeString();

            }
            catch (Exception)
            {

                returnstring = "Det lykkedes desværre ikke at afbooke tiden";

            }

            return returnstring;
        }


        [HttpPost]
        public string Booking(int RoomNumber, [Bind(Include = "ID")] WashTime currentTime)
        {
            WashTime currentbooking = db.WashTimes.Find(currentTime.ID); 
            if (currentbooking == null)
            {
                return "Det lykkedes desværre ikke at booke tiden";
            }

            currentbooking.IsBooked = true;
            currentbooking.RoomNumber = RoomNumber;
            db.Entry(currentbooking).State = EntityState.Modified;
            string returnstring = "";
            try
            {
                db.SaveChangesAsync();
                returnstring = "Det lykkedes " + currentbooking.Machine + " er booket til " +
                               currentbooking.Time.ToShortTimeString();

            }
            catch (Exception)
            {

                returnstring = "Det lykkedes desværre ikke at booke tiden";

            }

            return returnstring;

        }
    }
}
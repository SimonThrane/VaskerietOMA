using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using MyModel.WashTime;
using VaskerietOMA.DataAccess;

namespace VaskerietOMA.DatabaseFunction
{
    public class DatabaseFiller
    {
        private WashContext db = new WashContext(); 

        public void FillDay(DateTime day, string machine)
        {
            var tempDB = db.WashTimes.ToList();
            List<WashTime> temp = new List<WashTime>();

            for (int i = 6; i < 24; i++)
            {
                if (!tempDB.Any(
                        t =>
                            t.Time.Hour == i && t.Time.Date == day.Date && t.Machine == machine))
                {
                    day = day.AddHours(i);

                    temp.Add(
                        new WashTime
                        {
                            IsBooked = false,
                            Machine = machine,
                            Time = day,
                            RoomNumber = 0
                        }
                        );
                }
            }


            db.WashTimes.AddRange(temp);
            db.SaveChanges();
        }


        public void FillDatabaseWeek(string machine)
        {
            var tempDB = db.WashTimes.ToList();
            List<WashTime> temp = new List<WashTime>();
            for (int j = 0; j < 7; j++)
            {
                for (int i = 6; i < 24; i++)
                {
                    if (!tempDB.Any(
                            t =>
                                t.Time.Hour == i && HelperFunctions.GetIso8601WeekOfYear(t.Time) == HelperFunctions.GetIso8601WeekOfYear(DateTime.Today) &&
                                (int)t.Time.DayOfWeek == j && t.Machine == machine))
                    {
                        DateTime firstdayInWeek = HelperFunctions.GetFirstDayOfWeek(DateTime.Today, CultureInfo.CurrentCulture);
                        DateTime tempday = firstdayInWeek.AddDays(j);
                        var day = tempday.AddHours(i);

                        temp.Add(
                            new WashTime
                            {
                                IsBooked = false,
                                Machine = machine,
                                Time = day,
                                RoomNumber = 0
                            }
                            );
                    }
                }

            }
            db.WashTimes.AddRange(temp);
            db.SaveChanges();
        }
    }
}
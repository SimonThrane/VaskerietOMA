using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MyModel.WashTime;
using VaskerietOMA.DataAccess;
using VaskerietOMA.DatabaseFunction;
using VaskerietOMA.Models;

namespace VaskerietOMA.ViewModel
{
    public class TimeTableViewModel
    {
        public WashDayViewModel Monday { get; set; }
        public WashDayViewModel Tuesday { get; set; }
        public WashDayViewModel Wednesday { get; set; }
        public WashDayViewModel Thursday { get; set; }
        public WashDayViewModel Friday { get; set; }
        public WashDayViewModel Saturday { get; set; }
        public WashDayViewModel Sunday { get; set; }
        public int WeekNumber { get; set; }
        public BookingViewModel User { get; set; }
        private DatabaseFiller filler = new DatabaseFiller();


        private void FillWeek()
        {
            filler.FillDatabaseWeek("Left");
            filler.FillDatabaseWeek("Right");

        }

        public TimeTableViewModel()
        {
            var db = new WashContext();
            List<WashTime> times = db.WashTimes.Where(x => x.Time.Year == DateTime.Today.Year).ToList();
            List<WashTime> thisWeek =
                times.Where(
                    time =>
                        HelperFunctions.GetIso8601WeekOfYear(time.Time) ==
                        HelperFunctions.GetIso8601WeekOfYear(DateTime.Today)
                ).OrderBy(f => f.Time.Date).ThenBy(f => f.Time.Hour).ThenBy(f => f.Machine
                ).GroupBy(
                    f => new {f.Time.Date, f.Time.Hour, f.Machine}
                ).Select(
                    group => group.First()
                ).ToList();

            if (thisWeek.Count < (126*2))
            {
                FillWeek();
                times = db.WashTimes.Where(x => x.Time.Year == DateTime.Today.Year).ToList();
                thisWeek =
                    times.Where(
                        time =>
                            HelperFunctions.GetIso8601WeekOfYear(time.Time) ==
                            HelperFunctions.GetIso8601WeekOfYear(DateTime.Today)).ToList();
            }

            WeekNumber = HelperFunctions.GetIso8601WeekOfYear(DateTime.Now);
        }

        public TimeTableViewModel(List<WashTime> times)
        {
            Monday = new WashDayViewModel(times.GetRange(0, 36));
            Tuesday = new WashDayViewModel(times.GetRange(35, 36));
            Wednesday = new WashDayViewModel(times.GetRange(71, 36));
            Thursday = new WashDayViewModel(times.GetRange(107, 36));
            Friday = new WashDayViewModel(times.GetRange(143, 36));
            Saturday = new WashDayViewModel(times.GetRange(179, 36));
            Sunday = new WashDayViewModel(times.GetRange(215, 36));


            var el = times.FirstOrDefault();
            if (el != null)
                WeekNumber = HelperFunctions.GetIso8601WeekOfYear(el.Time);
            else
            {
                WeekNumber = HelperFunctions.GetIso8601WeekOfYear(DateTime.Now);
            }
        }

        public TimeTableViewModel(DateTime day)
        {
            WeekNumber = HelperFunctions.GetIso8601WeekOfYear(day);
            var db = new WashContext();
            List<WashTime> times = db.WashTimes.Where(x => x.Time.Year == day.Year).ToList();
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

            if (thisWeek.Count < (126 * 2))
            {
                FillWeek(day, db);
                times = db.WashTimes.Where(x => x.Time.Year == day.Year).ToList();
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

            Monday = new WashDayViewModel(thisWeek.GetRange(0, 36));
            Tuesday = new WashDayViewModel(thisWeek.GetRange(35, 36));
            Wednesday = new WashDayViewModel(thisWeek.GetRange(71, 36));
            Thursday = new WashDayViewModel(thisWeek.GetRange(107, 36));
            Friday = new WashDayViewModel(thisWeek.GetRange(143, 36));
            Saturday = new WashDayViewModel(thisWeek.GetRange(179, 36));
            Sunday = new WashDayViewModel(thisWeek.GetRange(215, 36));

        }

        private void FillWeek(DateTime day, WashContext  db)
        {
            DateTime firstdayInWeek = HelperFunctions.GetFirstDayOfWeek(day, CultureInfo.CurrentCulture);
            for (int i = 0; i < 7; i++)
            {
                filler.FillDay(firstdayInWeek.AddDays(i), "Left");
                filler.FillDay(firstdayInWeek.AddDays(i), "Right");

            }
        }
    }
}
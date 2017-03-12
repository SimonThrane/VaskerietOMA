using System;
using System.Collections.Generic;
using System.Linq;
using MyModel.WashTime;
using VaskerietOMA.DataAccess;
using VaskerietOMA.DatabaseFunction;
using VaskerietOMA.Models;

namespace VaskerietOMA.ViewModel
{
    public class TimeTableViewModel
    {
        public WashDayViewModel Monday;
        public WashDayViewModel Tuesday;
        public WashDayViewModel Wednesday;
        public WashDayViewModel Thursday;
        public WashDayViewModel Friday;
        public WashDayViewModel Saturday;
        public WashDayViewModel Sunday;
        public int WeekNumber;
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
    }
}
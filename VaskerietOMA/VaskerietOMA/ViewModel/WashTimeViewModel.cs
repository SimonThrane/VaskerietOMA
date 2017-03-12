using System;
using MyModel.WashTime;

namespace VaskerietOMA.ViewModel
{
    public class WashTimeViewModel
    {
        public int ID { get; set; }

        public DateTime Time { get; set; }

        public string TimeString { get; set; }

        public bool IsBooked { get; set; }

        public string Machine { get; set; }

        public int RoomNumber { get; set; }

        public WashTimeViewModel()
        {
            
        }

        public WashTimeViewModel(WashTime washTime)
        {
            ID = washTime.ID;
            Time = washTime.Time;
            TimeString = Time.ToShortTimeString();
            IsBooked = washTime.IsBooked;
            Machine = washTime.Machine;
            RoomNumber = washTime.RoomNumber;

        }
    }
}
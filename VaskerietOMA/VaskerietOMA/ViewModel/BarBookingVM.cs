using System;
using MyModel.WashTime;

namespace VaskerietOMA.ViewModel
{
    public class BarBookingVM
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Organizer { get; set; }
        public bool IsPaid { get; set; }
        public string Message { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public bool IsPublic { get; set; }

        public BarBookingVM()
        {}

        public BarBookingVM(BarBooking barBooking)
        {
            Id = barBooking.Id;
            StartTime = barBooking.StartTime;
            EndTime = barBooking.EndTime;
            Organizer = barBooking.Organizer;
            IsPaid = barBooking.IsPaid;
            Message = barBooking.Message;
            Name = barBooking.Name;
            IsPublic = barBooking.IsPublic;

        }

        public BarBooking ToData()
        {
            BarBooking b1 = new BarBooking();
            b1.Name = Name;
            b1.EndTime = EndTime;
            b1.Message = Message;
            b1.Organizer = Organizer;
            b1.StartTime = StartTime;
            b1.IsPublic = IsPublic;
            return b1;
        }
    }
}
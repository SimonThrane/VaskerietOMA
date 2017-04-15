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
        public BarBookingStatus Status { get; set; }
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
            Email = barBooking.Email;
            Status = (BarBookingStatus) barBooking.Status;


        }

        public BarBooking ToData()
        {
            BarBooking b1 = new BarBooking
            {
                Name = Name,
                EndTime = EndTime,
                Message = Message,
                Organizer = Organizer,
                StartTime = StartTime,
                IsPublic = IsPublic,
                Email = Email,
                Status = (int) Status
            };
            return b1;
        }
    }

    public enum BarBookingStatus
    {
        NotBooked,
        BookedWaiting,
        BookingApproved
    };
}
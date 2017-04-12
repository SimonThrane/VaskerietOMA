using System;

namespace MyModel.WashTime
{
    public class BarBooking
    {
        public int Id { get; set; }
        
        public string Name { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }
       
        public string Organizer { get; set; }

        public bool IsPaid { get; set; }

        public bool IsPublic { get; set; }

        public string Message { get; set; }

        
    }
}
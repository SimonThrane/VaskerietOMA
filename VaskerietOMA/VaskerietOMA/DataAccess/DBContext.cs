using MyModel.WashTime;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;


namespace VaskerietOMA.DataAccess
{
    public class WashContext : DbContext
    {
        public DbSet<WashTime> WashTimes { get; set; }
        public DbSet<BarBooking> BarBookings { get; set; }

        public WashContext()
            : base("DefaultConnection")
        {

        }
    }
}
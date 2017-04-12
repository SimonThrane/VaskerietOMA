using MyModel.WashTime;

namespace VaskerietOMA.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<VaskerietOMA.DataAccess.WashContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(VaskerietOMA.DataAccess.WashContext context)
        {

            var booking1 = new BarBooking
            {
                Name = "OMA Test1",
                EndTime = DateTime.MaxValue,
                StartTime = DateTime.Now,
                Message = "Rigeligt med øl",
                Organizer = "Værelse 14",
                IsPublic = true
            };
            var booking2 = new BarBooking
            {
                Name = "OMA Test2",
                EndTime = DateTime.MaxValue,
                StartTime = DateTime.Now.AddDays(2),
                Message = "Rigeligt med katte",
                Organizer = "Værelse 34",
                IsPublic = true
            };
            var booking3 = new BarBooking
            {
                Name = "OMA Test3",
                EndTime = DateTime.MaxValue,
                StartTime = DateTime.Now.AddDays(5),
                Message = "Rigeligt med vin",
                Organizer = "Værelse 45",
                IsPublic = false
            };
            context.BarBookings.AddOrUpdate(booking1);
            context.BarBookings.AddOrUpdate(booking2);
            context.BarBookings.AddOrUpdate(booking3);
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }
    }
}

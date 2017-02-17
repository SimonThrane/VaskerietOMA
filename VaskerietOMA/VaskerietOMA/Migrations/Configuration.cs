namespace VaskerietOMA.Migrations
{
    using MyModel.WashTime;
    using System;
    using System.Collections.Generic;
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
            List<WashTime> times = new List<WashTime>();
            for(int i =6; i<24;i++)
            {
                times.Add(new WashTime
                {
                    IsBooked = true,
                    Machine = "Rigth",
                    Time = new DateTime(2017, 01, 23, i, 0, 0)

                });
            }
            context.WashTimes.AddRange(times);
            
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

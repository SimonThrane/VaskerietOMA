namespace VaskerietOMA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedBarBooking : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BarBookings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        StartTime = c.DateTime(nullable: false),
                        EndTime = c.DateTime(nullable: false),
                        Organizer = c.String(),
                        IsPaid = c.Boolean(nullable: false),
                        Message = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.BarBookings");
        }
    }
}

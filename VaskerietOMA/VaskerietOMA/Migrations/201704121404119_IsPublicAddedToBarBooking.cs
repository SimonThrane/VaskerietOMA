namespace VaskerietOMA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IsPublicAddedToBarBooking : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BarBookings", "IsPublic", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.BarBookings", "IsPublic");
        }
    }
}

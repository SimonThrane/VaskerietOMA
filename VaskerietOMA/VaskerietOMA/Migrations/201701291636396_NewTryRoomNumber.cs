namespace VaskerietOMA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NewTryRoomNumber : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.WashTimes", "RoomNumber", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.WashTimes", "RoomNumber");
        }
    }
}

namespace JCarrollOnlineV2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedMicroPostNotificationFields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ApplicationUsers", "MicroPostEmailNotifications", c => c.Boolean(nullable: false));
            AddColumn("dbo.ApplicationUsers", "MicroPostSMSNotifications", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ApplicationUsers", "MicroPostSMSNotifications");
            DropColumn("dbo.ApplicationUsers", "MicroPostEmailNotifications");
        }
    }
}

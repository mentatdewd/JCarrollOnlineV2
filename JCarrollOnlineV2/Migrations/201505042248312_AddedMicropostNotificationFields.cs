namespace JCarrollOnlineV2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedMicropostNotificationFields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ApplicationUsers", "MicropostEmailNotifications", c => c.Boolean(nullable: false));
            AddColumn("dbo.ApplicationUsers", "MicropostSMSNotifications", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ApplicationUsers", "MicropostSMSNotifications");
            DropColumn("dbo.ApplicationUsers", "MicropostEmailNotifications");
        }
    }
}

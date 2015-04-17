namespace JCarrollOnlineV2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedMicropostModelUserIdFromIntToString : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Microposts", "UserId", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Microposts", "UserId", c => c.Int(nullable: false));
        }
    }
}

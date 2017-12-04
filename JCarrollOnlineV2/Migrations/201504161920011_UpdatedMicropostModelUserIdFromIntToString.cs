namespace JCarrollOnlineV2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedMicroPostModelUserIdFromIntToString : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.MicroPosts", "UserId", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.MicroPosts", "UserId", c => c.Int(nullable: false));
        }
    }
}

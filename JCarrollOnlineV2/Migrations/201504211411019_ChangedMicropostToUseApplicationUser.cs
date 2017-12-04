namespace JCarrollOnlineV2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangedMicroPostToUseApplicationUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MicroPosts", "User_Id", c => c.String(maxLength: 128));
            AlterColumn("dbo.ForumThreadEntries", "Title", c => c.String(maxLength: 255));
            CreateIndex("dbo.MicroPosts", "User_Id");
            AddForeignKey("dbo.MicroPosts", "User_Id", "dbo.ApplicationUsers", "Id");
            DropColumn("dbo.MicroPosts", "UserId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.MicroPosts", "UserId", c => c.String());
            DropForeignKey("dbo.MicroPosts", "User_Id", "dbo.ApplicationUsers");
            DropIndex("dbo.MicroPosts", new[] { "User_Id" });
            AlterColumn("dbo.ForumThreadEntries", "Title", c => c.String());
            DropColumn("dbo.MicroPosts", "User_Id");
        }
    }
}

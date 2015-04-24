namespace JCarrollOnlineV2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangedMicropostToUseApplicationUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Microposts", "User_Id", c => c.String(maxLength: 128));
            AlterColumn("dbo.ForumThreadEntries", "Title", c => c.String(maxLength: 255));
            CreateIndex("dbo.Microposts", "User_Id");
            AddForeignKey("dbo.Microposts", "User_Id", "dbo.ApplicationUsers", "Id");
            DropColumn("dbo.Microposts", "UserId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Microposts", "UserId", c => c.String());
            DropForeignKey("dbo.Microposts", "User_Id", "dbo.ApplicationUsers");
            DropIndex("dbo.Microposts", new[] { "User_Id" });
            AlterColumn("dbo.ForumThreadEntries", "Title", c => c.String());
            DropColumn("dbo.Microposts", "User_Id");
        }
    }
}

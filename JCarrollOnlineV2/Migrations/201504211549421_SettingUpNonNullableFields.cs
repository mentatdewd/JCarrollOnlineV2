namespace JCarrollOnlineV2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SettingUpNonNullableFields : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ForumThreadEntries", "Forum_Id", "dbo.Fora");
            DropForeignKey("dbo.ForumThreadEntries", "Author_Id", "dbo.ApplicationUsers");
            DropForeignKey("dbo.Relationships", "ApplicationUser_Id", "dbo.ApplicationUsers");
            DropForeignKey("dbo.Relationships", "Micropost_Id2", "dbo.Microposts");
            DropIndex("dbo.ForumThreadEntries", new[] { "Author_Id" });
            DropIndex("dbo.ForumThreadEntries", new[] { "Forum_Id" });
            DropIndex("dbo.Relationships", "IX_FirstAndSecond");
            DropIndex("dbo.Relationships", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.Relationships", new[] { "Micropost_Id2" });
            DropIndex("dbo.Microposts", new[] { "User_Id" });
            AlterColumn("dbo.ForumThreadEntries", "Author_Id", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.ForumThreadEntries", "Forum_Id", c => c.Int(nullable: false));
            AlterColumn("dbo.Relationships", "FollowerId", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.Relationships", "FollowedId", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.Relationships", "ApplicationUser_Id", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.Relationships", "Micropost_Id2", c => c.Int(nullable: false));
            AlterColumn("dbo.Microposts", "User_Id", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.ForumThreadEntries", "Author_Id");
            CreateIndex("dbo.ForumThreadEntries", "Forum_Id");
            CreateIndex("dbo.Relationships", new[] { "FollowerId", "FollowedId" }, unique: true, name: "IX_FirstAndSecond");
            CreateIndex("dbo.Relationships", "ApplicationUser_Id");
            CreateIndex("dbo.Relationships", "Micropost_Id2");
            CreateIndex("dbo.Microposts", "User_Id");
            AddForeignKey("dbo.ForumThreadEntries", "Forum_Id", "dbo.Fora", "Id", cascadeDelete: true);
            AddForeignKey("dbo.ForumThreadEntries", "Author_Id", "dbo.ApplicationUsers", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Relationships", "ApplicationUser_Id", "dbo.ApplicationUsers", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Relationships", "Micropost_Id2", "dbo.Microposts", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Relationships", "Micropost_Id2", "dbo.Microposts");
            DropForeignKey("dbo.Relationships", "ApplicationUser_Id", "dbo.ApplicationUsers");
            DropForeignKey("dbo.ForumThreadEntries", "Author_Id", "dbo.ApplicationUsers");
            DropForeignKey("dbo.ForumThreadEntries", "Forum_Id", "dbo.Fora");
            DropIndex("dbo.Microposts", new[] { "User_Id" });
            DropIndex("dbo.Relationships", new[] { "Micropost_Id2" });
            DropIndex("dbo.Relationships", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.Relationships", "IX_FirstAndSecond");
            DropIndex("dbo.ForumThreadEntries", new[] { "Forum_Id" });
            DropIndex("dbo.ForumThreadEntries", new[] { "Author_Id" });
            AlterColumn("dbo.Microposts", "User_Id", c => c.String(maxLength: 128));
            AlterColumn("dbo.Relationships", "Micropost_Id2", c => c.Int());
            AlterColumn("dbo.Relationships", "ApplicationUser_Id", c => c.String(maxLength: 128));
            AlterColumn("dbo.Relationships", "FollowedId", c => c.String(maxLength: 128));
            AlterColumn("dbo.Relationships", "FollowerId", c => c.String(maxLength: 128));
            AlterColumn("dbo.ForumThreadEntries", "Forum_Id", c => c.Int());
            AlterColumn("dbo.ForumThreadEntries", "Author_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.Microposts", "User_Id");
            CreateIndex("dbo.Relationships", "Micropost_Id2");
            CreateIndex("dbo.Relationships", "ApplicationUser_Id");
            CreateIndex("dbo.Relationships", new[] { "FollowerId", "FollowedId" }, unique: true, name: "IX_FirstAndSecond");
            CreateIndex("dbo.ForumThreadEntries", "Forum_Id");
            CreateIndex("dbo.ForumThreadEntries", "Author_Id");
            AddForeignKey("dbo.Relationships", "Micropost_Id2", "dbo.Microposts", "Id");
            AddForeignKey("dbo.Relationships", "ApplicationUser_Id", "dbo.ApplicationUsers", "Id");
            AddForeignKey("dbo.ForumThreadEntries", "Author_Id", "dbo.ApplicationUsers", "Id");
            AddForeignKey("dbo.ForumThreadEntries", "Forum_Id", "dbo.Fora", "Id");
        }
    }
}
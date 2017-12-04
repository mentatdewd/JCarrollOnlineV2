namespace JCarrollOnlineV2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LargeUpdate : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Relationships", "ApplicationUser_Id", "dbo.ApplicationUsers");
            DropForeignKey("dbo.Relationships", "ApplicationUser_Id1", "dbo.ApplicationUsers");
            DropForeignKey("dbo.Relationships", "ApplicationUser_Id2", "dbo.ApplicationUsers");
            DropForeignKey("dbo.Relationships", "MicroPost_Id", "dbo.MicroPosts");
            DropForeignKey("dbo.Relationships", "MicroPost_Id1", "dbo.MicroPosts");
            DropForeignKey("dbo.Relationships", "MicroPost_Id2", "dbo.MicroPosts");
            DropForeignKey("dbo.ForumThreadEntries", "ForumId", "dbo.Fora");
            DropForeignKey("dbo.ForumModerators", "ForumId", "dbo.Fora");
            DropIndex("dbo.ForumThreadEntries", new[] { "ForumId" });
            DropIndex("dbo.Relationships", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.Relationships", new[] { "ApplicationUser_Id1" });
            DropIndex("dbo.Relationships", new[] { "ApplicationUser_Id2" });
            DropIndex("dbo.Relationships", new[] { "MicroPost_Id" });
            DropIndex("dbo.Relationships", new[] { "MicroPost_Id1" });
            DropIndex("dbo.Relationships", new[] { "MicroPost_Id2" });
            RenameColumn(table: "dbo.ForumThreadEntries", name: "ApplicationUser_Id", newName: "Author_Id");
            RenameColumn(table: "dbo.ForumThreadEntries", name: "ForumId", newName: "Forum_Id");
            RenameIndex(table: "dbo.ForumThreadEntries", name: "IX_ApplicationUser_Id", newName: "IX_Author_Id");
            DropPrimaryKey("dbo.Fora");
            DropColumn("dbo.Fora", "ForumId");
            AddColumn("dbo.Fora", "Id", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.ForumThreadEntries", "Title", c => c.String());
            AlterColumn("dbo.ForumThreadEntries", "Forum_Id", c => c.Int());
            AddPrimaryKey("dbo.Fora", "Id");
            CreateIndex("dbo.ForumThreadEntries", "Forum_Id");
            AddForeignKey("dbo.ForumThreadEntries", "Forum_Id", "dbo.Fora", "Id");
            AddForeignKey("dbo.ForumModerators", "ForumId", "dbo.Fora", "Id", cascadeDelete: true);
            DropColumn("dbo.ForumThreadEntries", "AuthorId");
            DropTable("dbo.Relationships");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Relationships",
                c => new
                    {
                        FollowerId = c.String(nullable: false, maxLength: 128),
                        FollowedId = c.String(nullable: false, maxLength: 128),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                        ApplicationUser_Id = c.String(maxLength: 128),
                        ApplicationUser_Id1 = c.String(maxLength: 128),
                        ApplicationUser_Id2 = c.String(nullable: false, maxLength: 128),
                        MicroPost_Id = c.Int(nullable: false),
                        MicroPost_Id1 = c.Int(),
                        MicroPost_Id2 = c.Int(),
                    })
                .PrimaryKey(t => new { t.FollowerId, t.FollowedId });
            
            AddColumn("dbo.ForumThreadEntries", "AuthorId", c => c.String());
            AddColumn("dbo.Fora", "ForumId", c => c.Int(nullable: false, identity: true));
            DropForeignKey("dbo.ForumModerators", "ForumId", "dbo.Fora");
            DropForeignKey("dbo.ForumThreadEntries", "Forum_Id", "dbo.Fora");
            DropIndex("dbo.ForumThreadEntries", new[] { "Forum_Id" });
            DropPrimaryKey("dbo.Fora");
            AlterColumn("dbo.ForumThreadEntries", "Forum_Id", c => c.Int(nullable: false));
            AlterColumn("dbo.ForumThreadEntries", "Title", c => c.String(maxLength: 256));
            DropColumn("dbo.Fora", "Id");
            AddPrimaryKey("dbo.Fora", "ForumId");
            RenameIndex(table: "dbo.ForumThreadEntries", name: "IX_Author_Id", newName: "IX_ApplicationUser_Id");
            RenameColumn(table: "dbo.ForumThreadEntries", name: "Forum_Id", newName: "ForumId");
            RenameColumn(table: "dbo.ForumThreadEntries", name: "Author_Id", newName: "ApplicationUser_Id");
            CreateIndex("dbo.Relationships", "MicroPost_Id2");
            CreateIndex("dbo.Relationships", "MicroPost_Id1");
            CreateIndex("dbo.Relationships", "MicroPost_Id");
            CreateIndex("dbo.Relationships", "ApplicationUser_Id2");
            CreateIndex("dbo.Relationships", "ApplicationUser_Id1");
            CreateIndex("dbo.Relationships", "ApplicationUser_Id");
            CreateIndex("dbo.ForumThreadEntries", "ForumId");
            AddForeignKey("dbo.ForumModerators", "ForumId", "dbo.Fora", "ForumId", cascadeDelete: true);
            AddForeignKey("dbo.ForumThreadEntries", "ForumId", "dbo.Fora", "ForumId", cascadeDelete: true);
            AddForeignKey("dbo.Relationships", "MicroPost_Id2", "dbo.MicroPosts", "Id");
            AddForeignKey("dbo.Relationships", "MicroPost_Id1", "dbo.MicroPosts", "Id");
            AddForeignKey("dbo.Relationships", "MicroPost_Id", "dbo.MicroPosts", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Relationships", "ApplicationUser_Id2", "dbo.ApplicationUsers", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Relationships", "ApplicationUser_Id1", "dbo.ApplicationUsers", "Id");
            AddForeignKey("dbo.Relationships", "ApplicationUser_Id", "dbo.ApplicationUsers", "Id");
        }
    }
}

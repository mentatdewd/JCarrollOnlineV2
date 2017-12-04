namespace JCarrollOnlineV2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class StillPlayingWithManyToManyMicroPostsUsers : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ApplicationUserMicroPosts", "ApplicationUser_Id", "dbo.ApplicationUsers");
            DropForeignKey("dbo.ApplicationUserMicroPosts", "MicroPost_Id", "dbo.MicroPosts");
            DropForeignKey("dbo.ApplicationUserMicroPost1", "ApplicationUser_Id", "dbo.ApplicationUsers");
            DropForeignKey("dbo.ApplicationUserMicroPost1", "MicroPost_Id", "dbo.MicroPosts");
            DropIndex("dbo.ApplicationUserMicroPosts", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.ApplicationUserMicroPosts", new[] { "MicroPost_Id" });
            DropIndex("dbo.ApplicationUserMicroPost1", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.ApplicationUserMicroPost1", new[] { "MicroPost_Id" });
            DropPrimaryKey("dbo.Relationships");
            AddColumn("dbo.Relationships", "ApplicationUser_Id", c => c.String(maxLength: 128));
            AddColumn("dbo.Relationships", "ApplicationUser_Id1", c => c.String(maxLength: 128));
            AddColumn("dbo.Relationships", "ApplicationUser_Id2", c => c.String(nullable: false, maxLength: 128));
            AddColumn("dbo.Relationships", "MicroPost_Id", c => c.Int(nullable: false));
            AddColumn("dbo.Relationships", "MicroPost_Id1", c => c.Int());
            AddColumn("dbo.Relationships", "MicroPost_Id2", c => c.Int());
            AlterColumn("dbo.Relationships", "FollowerId", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.Relationships", "FollowedId", c => c.String(nullable: false, maxLength: 128));
            AddPrimaryKey("dbo.Relationships", new[] { "FollowerId", "FollowedId" });
            CreateIndex("dbo.Relationships", "ApplicationUser_Id");
            CreateIndex("dbo.Relationships", "ApplicationUser_Id1");
            CreateIndex("dbo.Relationships", "ApplicationUser_Id2");
            CreateIndex("dbo.Relationships", "MicroPost_Id");
            CreateIndex("dbo.Relationships", "MicroPost_Id1");
            CreateIndex("dbo.Relationships", "MicroPost_Id2");
            AddForeignKey("dbo.Relationships", "ApplicationUser_Id", "dbo.ApplicationUsers", "Id");
            AddForeignKey("dbo.Relationships", "ApplicationUser_Id1", "dbo.ApplicationUsers", "Id");
            AddForeignKey("dbo.Relationships", "ApplicationUser_Id2", "dbo.ApplicationUsers", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Relationships", "MicroPost_Id", "dbo.MicroPosts", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Relationships", "MicroPost_Id1", "dbo.MicroPosts", "Id");
            AddForeignKey("dbo.Relationships", "MicroPost_Id2", "dbo.MicroPosts", "Id");
            DropColumn("dbo.Relationships", "Id");
            DropTable("dbo.ApplicationUserMicroPosts");
            DropTable("dbo.ApplicationUserMicroPost1");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.ApplicationUserMicroPost1",
                c => new
                    {
                        ApplicationUser_Id = c.String(nullable: false, maxLength: 128),
                        MicroPost_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ApplicationUser_Id, t.MicroPost_Id });
            
            CreateTable(
                "dbo.ApplicationUserMicroPosts",
                c => new
                    {
                        ApplicationUser_Id = c.String(nullable: false, maxLength: 128),
                        MicroPost_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ApplicationUser_Id, t.MicroPost_Id });
            
            AddColumn("dbo.Relationships", "Id", c => c.Int(nullable: false, identity: true));
            DropForeignKey("dbo.Relationships", "MicroPost_Id2", "dbo.MicroPosts");
            DropForeignKey("dbo.Relationships", "MicroPost_Id1", "dbo.MicroPosts");
            DropForeignKey("dbo.Relationships", "MicroPost_Id", "dbo.MicroPosts");
            DropForeignKey("dbo.Relationships", "ApplicationUser_Id2", "dbo.ApplicationUsers");
            DropForeignKey("dbo.Relationships", "ApplicationUser_Id1", "dbo.ApplicationUsers");
            DropForeignKey("dbo.Relationships", "ApplicationUser_Id", "dbo.ApplicationUsers");
            DropIndex("dbo.Relationships", new[] { "MicroPost_Id2" });
            DropIndex("dbo.Relationships", new[] { "MicroPost_Id1" });
            DropIndex("dbo.Relationships", new[] { "MicroPost_Id" });
            DropIndex("dbo.Relationships", new[] { "ApplicationUser_Id2" });
            DropIndex("dbo.Relationships", new[] { "ApplicationUser_Id1" });
            DropIndex("dbo.Relationships", new[] { "ApplicationUser_Id" });
            DropPrimaryKey("dbo.Relationships");
            AlterColumn("dbo.Relationships", "FollowedId", c => c.String());
            AlterColumn("dbo.Relationships", "FollowerId", c => c.String());
            DropColumn("dbo.Relationships", "MicroPost_Id2");
            DropColumn("dbo.Relationships", "MicroPost_Id1");
            DropColumn("dbo.Relationships", "MicroPost_Id");
            DropColumn("dbo.Relationships", "ApplicationUser_Id2");
            DropColumn("dbo.Relationships", "ApplicationUser_Id1");
            DropColumn("dbo.Relationships", "ApplicationUser_Id");
            AddPrimaryKey("dbo.Relationships", "Id");
            CreateIndex("dbo.ApplicationUserMicroPost1", "MicroPost_Id");
            CreateIndex("dbo.ApplicationUserMicroPost1", "ApplicationUser_Id");
            CreateIndex("dbo.ApplicationUserMicroPosts", "MicroPost_Id");
            CreateIndex("dbo.ApplicationUserMicroPosts", "ApplicationUser_Id");
            AddForeignKey("dbo.ApplicationUserMicroPost1", "MicroPost_Id", "dbo.MicroPosts", "Id", cascadeDelete: true);
            AddForeignKey("dbo.ApplicationUserMicroPost1", "ApplicationUser_Id", "dbo.ApplicationUsers", "Id", cascadeDelete: true);
            AddForeignKey("dbo.ApplicationUserMicroPosts", "MicroPost_Id", "dbo.MicroPosts", "Id", cascadeDelete: true);
            AddForeignKey("dbo.ApplicationUserMicroPosts", "ApplicationUser_Id", "dbo.ApplicationUsers", "Id", cascadeDelete: true);
        }
    }
}

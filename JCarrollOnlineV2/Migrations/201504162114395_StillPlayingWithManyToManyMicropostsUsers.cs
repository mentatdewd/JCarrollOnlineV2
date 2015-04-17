namespace JCarrollOnlineV2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class StillPlayingWithManyToManyMicropostsUsers : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ApplicationUserMicroposts", "ApplicationUser_Id", "dbo.ApplicationUsers");
            DropForeignKey("dbo.ApplicationUserMicroposts", "Micropost_Id", "dbo.Microposts");
            DropForeignKey("dbo.ApplicationUserMicropost1", "ApplicationUser_Id", "dbo.ApplicationUsers");
            DropForeignKey("dbo.ApplicationUserMicropost1", "Micropost_Id", "dbo.Microposts");
            DropIndex("dbo.ApplicationUserMicroposts", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.ApplicationUserMicroposts", new[] { "Micropost_Id" });
            DropIndex("dbo.ApplicationUserMicropost1", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.ApplicationUserMicropost1", new[] { "Micropost_Id" });
            DropPrimaryKey("dbo.Relationships");
            AddColumn("dbo.Relationships", "ApplicationUser_Id", c => c.String(maxLength: 128));
            AddColumn("dbo.Relationships", "ApplicationUser_Id1", c => c.String(maxLength: 128));
            AddColumn("dbo.Relationships", "ApplicationUser_Id2", c => c.String(nullable: false, maxLength: 128));
            AddColumn("dbo.Relationships", "Micropost_Id", c => c.Int(nullable: false));
            AddColumn("dbo.Relationships", "Micropost_Id1", c => c.Int());
            AddColumn("dbo.Relationships", "Micropost_Id2", c => c.Int());
            AlterColumn("dbo.Relationships", "FollowerId", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.Relationships", "FollowedId", c => c.String(nullable: false, maxLength: 128));
            AddPrimaryKey("dbo.Relationships", new[] { "FollowerId", "FollowedId" });
            CreateIndex("dbo.Relationships", "ApplicationUser_Id");
            CreateIndex("dbo.Relationships", "ApplicationUser_Id1");
            CreateIndex("dbo.Relationships", "ApplicationUser_Id2");
            CreateIndex("dbo.Relationships", "Micropost_Id");
            CreateIndex("dbo.Relationships", "Micropost_Id1");
            CreateIndex("dbo.Relationships", "Micropost_Id2");
            AddForeignKey("dbo.Relationships", "ApplicationUser_Id", "dbo.ApplicationUsers", "Id");
            AddForeignKey("dbo.Relationships", "ApplicationUser_Id1", "dbo.ApplicationUsers", "Id");
            AddForeignKey("dbo.Relationships", "ApplicationUser_Id2", "dbo.ApplicationUsers", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Relationships", "Micropost_Id", "dbo.Microposts", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Relationships", "Micropost_Id1", "dbo.Microposts", "Id");
            AddForeignKey("dbo.Relationships", "Micropost_Id2", "dbo.Microposts", "Id");
            DropColumn("dbo.Relationships", "Id");
            DropTable("dbo.ApplicationUserMicroposts");
            DropTable("dbo.ApplicationUserMicropost1");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.ApplicationUserMicropost1",
                c => new
                    {
                        ApplicationUser_Id = c.String(nullable: false, maxLength: 128),
                        Micropost_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ApplicationUser_Id, t.Micropost_Id });
            
            CreateTable(
                "dbo.ApplicationUserMicroposts",
                c => new
                    {
                        ApplicationUser_Id = c.String(nullable: false, maxLength: 128),
                        Micropost_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ApplicationUser_Id, t.Micropost_Id });
            
            AddColumn("dbo.Relationships", "Id", c => c.Int(nullable: false, identity: true));
            DropForeignKey("dbo.Relationships", "Micropost_Id2", "dbo.Microposts");
            DropForeignKey("dbo.Relationships", "Micropost_Id1", "dbo.Microposts");
            DropForeignKey("dbo.Relationships", "Micropost_Id", "dbo.Microposts");
            DropForeignKey("dbo.Relationships", "ApplicationUser_Id2", "dbo.ApplicationUsers");
            DropForeignKey("dbo.Relationships", "ApplicationUser_Id1", "dbo.ApplicationUsers");
            DropForeignKey("dbo.Relationships", "ApplicationUser_Id", "dbo.ApplicationUsers");
            DropIndex("dbo.Relationships", new[] { "Micropost_Id2" });
            DropIndex("dbo.Relationships", new[] { "Micropost_Id1" });
            DropIndex("dbo.Relationships", new[] { "Micropost_Id" });
            DropIndex("dbo.Relationships", new[] { "ApplicationUser_Id2" });
            DropIndex("dbo.Relationships", new[] { "ApplicationUser_Id1" });
            DropIndex("dbo.Relationships", new[] { "ApplicationUser_Id" });
            DropPrimaryKey("dbo.Relationships");
            AlterColumn("dbo.Relationships", "FollowedId", c => c.String());
            AlterColumn("dbo.Relationships", "FollowerId", c => c.String());
            DropColumn("dbo.Relationships", "Micropost_Id2");
            DropColumn("dbo.Relationships", "Micropost_Id1");
            DropColumn("dbo.Relationships", "Micropost_Id");
            DropColumn("dbo.Relationships", "ApplicationUser_Id2");
            DropColumn("dbo.Relationships", "ApplicationUser_Id1");
            DropColumn("dbo.Relationships", "ApplicationUser_Id");
            AddPrimaryKey("dbo.Relationships", "Id");
            CreateIndex("dbo.ApplicationUserMicropost1", "Micropost_Id");
            CreateIndex("dbo.ApplicationUserMicropost1", "ApplicationUser_Id");
            CreateIndex("dbo.ApplicationUserMicroposts", "Micropost_Id");
            CreateIndex("dbo.ApplicationUserMicroposts", "ApplicationUser_Id");
            AddForeignKey("dbo.ApplicationUserMicropost1", "Micropost_Id", "dbo.Microposts", "Id", cascadeDelete: true);
            AddForeignKey("dbo.ApplicationUserMicropost1", "ApplicationUser_Id", "dbo.ApplicationUsers", "Id", cascadeDelete: true);
            AddForeignKey("dbo.ApplicationUserMicroposts", "Micropost_Id", "dbo.Microposts", "Id", cascadeDelete: true);
            AddForeignKey("dbo.ApplicationUserMicroposts", "ApplicationUser_Id", "dbo.ApplicationUsers", "Id", cascadeDelete: true);
        }
    }
}

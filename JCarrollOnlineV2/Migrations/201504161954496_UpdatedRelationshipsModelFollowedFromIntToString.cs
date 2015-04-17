namespace JCarrollOnlineV2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedRelationshipsModelFollowedFromIntToString : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Microposts", "ApplicationUser_Id", "dbo.ApplicationUsers");
            DropIndex("dbo.Microposts", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.Relationships", "FollowerIndex");
            DropIndex("dbo.Relationships", "FollowedIndex");
            DropIndex("dbo.Relationships", "FollowerAndFollowedIndex");
            CreateTable(
                "dbo.ApplicationUserMicroposts",
                c => new
                    {
                        ApplicationUser_Id = c.String(nullable: false, maxLength: 128),
                        Micropost_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ApplicationUser_Id, t.Micropost_Id })
                .ForeignKey("dbo.ApplicationUsers", t => t.ApplicationUser_Id, cascadeDelete: true)
                .ForeignKey("dbo.Microposts", t => t.Micropost_Id, cascadeDelete: true)
                .Index(t => t.ApplicationUser_Id)
                .Index(t => t.Micropost_Id);
            
            CreateTable(
                "dbo.ApplicationUserMicropost1",
                c => new
                    {
                        ApplicationUser_Id = c.String(nullable: false, maxLength: 128),
                        Micropost_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ApplicationUser_Id, t.Micropost_Id })
                .ForeignKey("dbo.ApplicationUsers", t => t.ApplicationUser_Id, cascadeDelete: true)
                .ForeignKey("dbo.Microposts", t => t.Micropost_Id, cascadeDelete: true)
                .Index(t => t.ApplicationUser_Id)
                .Index(t => t.Micropost_Id);
            
            AlterColumn("dbo.Relationships", "FollowerId", c => c.String());
            AlterColumn("dbo.Relationships", "FollowedId", c => c.String());
            DropColumn("dbo.Microposts", "ApplicationUser_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Microposts", "ApplicationUser_Id", c => c.String(maxLength: 128));
            DropForeignKey("dbo.ApplicationUserMicropost1", "Micropost_Id", "dbo.Microposts");
            DropForeignKey("dbo.ApplicationUserMicropost1", "ApplicationUser_Id", "dbo.ApplicationUsers");
            DropForeignKey("dbo.ApplicationUserMicroposts", "Micropost_Id", "dbo.Microposts");
            DropForeignKey("dbo.ApplicationUserMicroposts", "ApplicationUser_Id", "dbo.ApplicationUsers");
            DropIndex("dbo.ApplicationUserMicropost1", new[] { "Micropost_Id" });
            DropIndex("dbo.ApplicationUserMicropost1", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.ApplicationUserMicroposts", new[] { "Micropost_Id" });
            DropIndex("dbo.ApplicationUserMicroposts", new[] { "ApplicationUser_Id" });
            AlterColumn("dbo.Relationships", "FollowedId", c => c.Int(nullable: false));
            AlterColumn("dbo.Relationships", "FollowerId", c => c.Int(nullable: false));
            DropTable("dbo.ApplicationUserMicropost1");
            DropTable("dbo.ApplicationUserMicroposts");
            CreateIndex("dbo.Relationships", "FollowedId", unique: true, name: "FollowerAndFollowedIndex");
            CreateIndex("dbo.Relationships", "FollowedId", name: "FollowedIndex");
            CreateIndex("dbo.Relationships", "FollowerId", name: "FollowerIndex");
            CreateIndex("dbo.Microposts", "ApplicationUser_Id");
            AddForeignKey("dbo.Microposts", "ApplicationUser_Id", "dbo.ApplicationUsers", "Id");
        }
    }
}

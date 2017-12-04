namespace JCarrollOnlineV2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedRelationshipsModelFollowedFromIntToString : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.MicroPosts", "ApplicationUser_Id", "dbo.ApplicationUsers");
            DropIndex("dbo.MicroPosts", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.Relationships", "FollowerIndex");
            DropIndex("dbo.Relationships", "FollowedIndex");
            DropIndex("dbo.Relationships", "FollowerAndFollowedIndex");
            CreateTable(
                "dbo.ApplicationUserMicroPosts",
                c => new
                    {
                        ApplicationUser_Id = c.String(nullable: false, maxLength: 128),
                        MicroPost_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ApplicationUser_Id, t.MicroPost_Id })
                .ForeignKey("dbo.ApplicationUsers", t => t.ApplicationUser_Id, cascadeDelete: true)
                .ForeignKey("dbo.MicroPosts", t => t.MicroPost_Id, cascadeDelete: true)
                .Index(t => t.ApplicationUser_Id)
                .Index(t => t.MicroPost_Id);
            
            CreateTable(
                "dbo.ApplicationUserMicroPost1",
                c => new
                    {
                        ApplicationUser_Id = c.String(nullable: false, maxLength: 128),
                        MicroPost_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ApplicationUser_Id, t.MicroPost_Id })
                .ForeignKey("dbo.ApplicationUsers", t => t.ApplicationUser_Id, cascadeDelete: true)
                .ForeignKey("dbo.MicroPosts", t => t.MicroPost_Id, cascadeDelete: true)
                .Index(t => t.ApplicationUser_Id)
                .Index(t => t.MicroPost_Id);
            
            AlterColumn("dbo.Relationships", "FollowerId", c => c.String());
            AlterColumn("dbo.Relationships", "FollowedId", c => c.String());
            DropColumn("dbo.MicroPosts", "ApplicationUser_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.MicroPosts", "ApplicationUser_Id", c => c.String(maxLength: 128));
            DropForeignKey("dbo.ApplicationUserMicroPost1", "MicroPost_Id", "dbo.MicroPosts");
            DropForeignKey("dbo.ApplicationUserMicroPost1", "ApplicationUser_Id", "dbo.ApplicationUsers");
            DropForeignKey("dbo.ApplicationUserMicroPosts", "MicroPost_Id", "dbo.MicroPosts");
            DropForeignKey("dbo.ApplicationUserMicroPosts", "ApplicationUser_Id", "dbo.ApplicationUsers");
            DropIndex("dbo.ApplicationUserMicroPost1", new[] { "MicroPost_Id" });
            DropIndex("dbo.ApplicationUserMicroPost1", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.ApplicationUserMicroPosts", new[] { "MicroPost_Id" });
            DropIndex("dbo.ApplicationUserMicroPosts", new[] { "ApplicationUser_Id" });
            AlterColumn("dbo.Relationships", "FollowedId", c => c.Int(nullable: false));
            AlterColumn("dbo.Relationships", "FollowerId", c => c.Int(nullable: false));
            DropTable("dbo.ApplicationUserMicroPost1");
            DropTable("dbo.ApplicationUserMicroPosts");
            CreateIndex("dbo.Relationships", "FollowedId", unique: true, name: "FollowerAndFollowedIndex");
            CreateIndex("dbo.Relationships", "FollowedId", name: "FollowedIndex");
            CreateIndex("dbo.Relationships", "FollowerId", name: "FollowerIndex");
            CreateIndex("dbo.MicroPosts", "ApplicationUser_Id");
            AddForeignKey("dbo.MicroPosts", "ApplicationUser_Id", "dbo.ApplicationUsers", "Id");
        }
    }
}

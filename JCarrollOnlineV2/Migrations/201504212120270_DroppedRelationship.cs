namespace JCarrollOnlineV2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DroppedRelationship : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Relationships", "ApplicationUser_Id", "dbo.ApplicationUsers");
            DropForeignKey("dbo.ApplicationUsers", "Relationship_Id", "dbo.Relationships");
            DropForeignKey("dbo.ApplicationUsers", "Relationship_Id1", "dbo.Relationships");
            DropForeignKey("dbo.Relationships", "MicroPost", "dbo.MicroPosts");
            DropForeignKey("dbo.Relationships", "MicroPost1", "dbo.MicroPosts");
            DropForeignKey("dbo.Relationships", "MicroPost2", "dbo.MicroPosts");
            DropForeignKey("dbo.Relationships", "ApplicationUser_Id1", "dbo.ApplicationUsers");
            DropForeignKey("dbo.Relationships", "ApplicationUser_Id2", "dbo.ApplicationUsers");
            DropIndex("dbo.ApplicationUsers", new[] { "Relationship_Id" });
            DropIndex("dbo.ApplicationUsers", new[] { "Relationship_Id1" });
            DropIndex("dbo.Relationships", "IX_FirstAndSecond");
            DropIndex("dbo.Relationships", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.Relationships", new[] { "MicroPost" });
            DropIndex("dbo.Relationships", new[] { "MicroPost1" });
            DropIndex("dbo.Relationships", new[] { "MicroPost2" });
            DropIndex("dbo.Relationships", new[] { "ApplicationUser_Id1" });
            DropIndex("dbo.Relationships", new[] { "ApplicationUser_Id2" });
            RenameColumn(table: "dbo.MicroPosts", name: "User_Id", newName: "Author_Id");
            RenameIndex(table: "dbo.MicroPosts", name: "IX_User_Id", newName: "IX_Author_Id");
            CreateTable(
                "dbo.UserFollowers",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        FollowerId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.FollowerId })
                .ForeignKey("dbo.ApplicationUsers", t => t.UserId)
                .ForeignKey("dbo.ApplicationUsers", t => t.FollowerId)
                .Index(t => t.UserId)
                .Index(t => t.FollowerId);
            
            DropColumn("dbo.ApplicationUsers", "Relationship_Id");
            DropColumn("dbo.ApplicationUsers", "Relationship_Id1");
            DropTable("dbo.Relationships");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Relationships",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FollowerId = c.String(nullable: false, maxLength: 128),
                        FollowedId = c.String(nullable: false, maxLength: 128),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                        ApplicationUser_Id = c.String(nullable: false, maxLength: 128),
                        MicroPost = c.Int(),
                        MicroPost1 = c.Int(),
                        MicroPost2 = c.Int(nullable: false),
                        ApplicationUser_Id1 = c.String(maxLength: 128),
                        ApplicationUser_Id2 = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.ApplicationUsers", "Relationship_Id1", c => c.Int());
            AddColumn("dbo.ApplicationUsers", "Relationship_Id", c => c.Int());
            DropForeignKey("dbo.UserFollowers", "FollowerId", "dbo.ApplicationUsers");
            DropForeignKey("dbo.UserFollowers", "UserId", "dbo.ApplicationUsers");
            DropIndex("dbo.UserFollowers", new[] { "FollowerId" });
            DropIndex("dbo.UserFollowers", new[] { "UserId" });
            DropTable("dbo.UserFollowers");
            RenameIndex(table: "dbo.MicroPosts", name: "IX_Author_Id", newName: "IX_User_Id");
            RenameColumn(table: "dbo.MicroPosts", name: "Author_Id", newName: "User_Id");
            CreateIndex("dbo.Relationships", "ApplicationUser_Id2");
            CreateIndex("dbo.Relationships", "ApplicationUser_Id1");
            CreateIndex("dbo.Relationships", "MicroPost2");
            CreateIndex("dbo.Relationships", "MicroPost1");
            CreateIndex("dbo.Relationships", "MicroPost");
            CreateIndex("dbo.Relationships", "ApplicationUser_Id");
            CreateIndex("dbo.Relationships", new[] { "FollowerId", "FollowedId" }, unique: true, name: "IX_FirstAndSecond");
            CreateIndex("dbo.ApplicationUsers", "Relationship_Id1");
            CreateIndex("dbo.ApplicationUsers", "Relationship_Id");
            AddForeignKey("dbo.Relationships", "ApplicationUser_Id2", "dbo.ApplicationUsers", "Id");
            AddForeignKey("dbo.Relationships", "ApplicationUser_Id1", "dbo.ApplicationUsers", "Id");
            AddForeignKey("dbo.Relationships", "MicroPost2", "dbo.MicroPosts", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Relationships", "MicroPost1", "dbo.MicroPosts", "Id");
            AddForeignKey("dbo.Relationships", "MicroPost", "dbo.MicroPosts", "Id");
            AddForeignKey("dbo.ApplicationUsers", "Relationship_Id1", "dbo.Relationships", "Id");
            AddForeignKey("dbo.ApplicationUsers", "Relationship_Id", "dbo.Relationships", "Id");
            AddForeignKey("dbo.Relationships", "ApplicationUser_Id", "dbo.ApplicationUsers", "Id", cascadeDelete: true);
        }
    }
}

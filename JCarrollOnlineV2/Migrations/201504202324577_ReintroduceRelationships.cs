namespace JCarrollOnlineV2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReintroduceRelationships : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Relationships",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FollowerId = c.String(maxLength: 128),
                        FollowedId = c.String(maxLength: 128),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                        ApplicationUser_Id = c.String(maxLength: 128),
                        Micropost_Id = c.Int(),
                        Micropost_Id1 = c.Int(),
                        Micropost_Id2 = c.Int(),
                        ApplicationUser_Id1 = c.String(maxLength: 128),
                        ApplicationUser_Id2 = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ApplicationUsers", t => t.ApplicationUser_Id)
                .ForeignKey("dbo.Microposts", t => t.Micropost_Id)
                .ForeignKey("dbo.Microposts", t => t.Micropost_Id1)
                .ForeignKey("dbo.Microposts", t => t.Micropost_Id2)
                .ForeignKey("dbo.ApplicationUsers", t => t.ApplicationUser_Id1)
                .ForeignKey("dbo.ApplicationUsers", t => t.ApplicationUser_Id2)
                .Index(t => new { t.FollowerId, t.FollowedId }, unique: true, name: "IX_FirstAndSecond")
                .Index(t => t.ApplicationUser_Id)
                .Index(t => t.Micropost_Id)
                .Index(t => t.Micropost_Id1)
                .Index(t => t.Micropost_Id2)
                .Index(t => t.ApplicationUser_Id1)
                .Index(t => t.ApplicationUser_Id2);
            
            AddColumn("dbo.ApplicationUsers", "Relationship_Id", c => c.Int());
            AddColumn("dbo.ApplicationUsers", "Relationship_Id1", c => c.Int());
            CreateIndex("dbo.ApplicationUsers", "Relationship_Id");
            CreateIndex("dbo.ApplicationUsers", "Relationship_Id1");
            AddForeignKey("dbo.ApplicationUsers", "Relationship_Id", "dbo.Relationships", "Id");
            AddForeignKey("dbo.ApplicationUsers", "Relationship_Id1", "dbo.Relationships", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Relationships", "ApplicationUser_Id2", "dbo.ApplicationUsers");
            DropForeignKey("dbo.Relationships", "ApplicationUser_Id1", "dbo.ApplicationUsers");
            DropForeignKey("dbo.Relationships", "Micropost_Id2", "dbo.Microposts");
            DropForeignKey("dbo.Relationships", "Micropost_Id1", "dbo.Microposts");
            DropForeignKey("dbo.Relationships", "Micropost_Id", "dbo.Microposts");
            DropForeignKey("dbo.ApplicationUsers", "Relationship_Id1", "dbo.Relationships");
            DropForeignKey("dbo.ApplicationUsers", "Relationship_Id", "dbo.Relationships");
            DropForeignKey("dbo.Relationships", "ApplicationUser_Id", "dbo.ApplicationUsers");
            DropIndex("dbo.Relationships", new[] { "ApplicationUser_Id2" });
            DropIndex("dbo.Relationships", new[] { "ApplicationUser_Id1" });
            DropIndex("dbo.Relationships", new[] { "Micropost_Id2" });
            DropIndex("dbo.Relationships", new[] { "Micropost_Id1" });
            DropIndex("dbo.Relationships", new[] { "Micropost_Id" });
            DropIndex("dbo.Relationships", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.Relationships", "IX_FirstAndSecond");
            DropIndex("dbo.ApplicationUsers", new[] { "Relationship_Id1" });
            DropIndex("dbo.ApplicationUsers", new[] { "Relationship_Id" });
            DropColumn("dbo.ApplicationUsers", "Relationship_Id1");
            DropColumn("dbo.ApplicationUsers", "Relationship_Id");
            DropTable("dbo.Relationships");
        }
    }
}
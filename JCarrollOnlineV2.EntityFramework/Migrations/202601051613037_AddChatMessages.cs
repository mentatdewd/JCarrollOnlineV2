namespace JCarrollOnlineV2.EntityFramework.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddChatMessages : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ChatMessage",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Message = c.String(nullable: false, maxLength: 500),
                        AuthorId = c.String(nullable: false, maxLength: 128),
                        CreatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ApplicationUser", t => t.AuthorId, cascadeDelete: true)
                .Index(t => t.AuthorId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ChatMessage", "AuthorId", "dbo.ApplicationUser");
            DropIndex("dbo.ChatMessage", new[] { "AuthorId" });
            DropTable("dbo.ChatMessage");
        }
    }
}

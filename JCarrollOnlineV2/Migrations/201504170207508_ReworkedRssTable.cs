namespace JCarrollOnlineV2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReworkedRssTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RssFeedModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Author = c.String(),
                        Description = c.String(maxLength: 256),
                        Link = c.String(),
                        PublicationUtcTime = c.DateTime(nullable: false),
                        Title = c.String(),
                        UniqueLinkOrName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            DropTable("dbo.RssFeeds");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.RssFeeds",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 128),
                        Summary = c.String(maxLength: 256),
                        Url = c.String(),
                        PublishedAt = c.DateTime(nullable: false),
                        Guid = c.String(maxLength: 38),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            DropTable("dbo.RssFeedModels");
        }
    }
}

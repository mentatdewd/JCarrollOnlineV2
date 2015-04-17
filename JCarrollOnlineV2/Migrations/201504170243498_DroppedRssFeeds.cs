namespace JCarrollOnlineV2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DroppedRssFeeds : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.RssFeeds");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.RssFeeds",
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
            
        }
    }
}

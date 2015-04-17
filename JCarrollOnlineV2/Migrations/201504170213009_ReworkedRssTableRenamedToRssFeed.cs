namespace JCarrollOnlineV2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReworkedRssTableRenamedToRssFeed : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.RssFeedModels", newName: "RssFeeds");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.RssFeeds", newName: "RssFeedModels");
        }
    }
}

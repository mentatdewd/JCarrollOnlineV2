namespace JCarrollOnlineV2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addblogtitle : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BlogItem", "Title", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.BlogItem", "Title");
        }
    }
}

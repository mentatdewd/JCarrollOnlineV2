namespace JCarrollOnlineV2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangedRootIdToNullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ForumThreadEntries", "RootId", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ForumThreadEntries", "RootId", c => c.Int(nullable: false));
        }
    }
}

namespace JCarrollOnlineV2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SetMostFieldsInBlogItemToRequired : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.BlogItems", "Author_Id", "dbo.ApplicationUsers");
            DropIndex("dbo.BlogItems", new[] { "Author_Id" });
            AlterColumn("dbo.BlogItems", "Content", c => c.String(nullable: false));
            AlterColumn("dbo.BlogItems", "Author_Id", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.BlogItems", "Author_Id");
            AddForeignKey("dbo.BlogItems", "Author_Id", "dbo.ApplicationUsers", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BlogItems", "Author_Id", "dbo.ApplicationUsers");
            DropIndex("dbo.BlogItems", new[] { "Author_Id" });
            AlterColumn("dbo.BlogItems", "Author_Id", c => c.String(maxLength: 128));
            AlterColumn("dbo.BlogItems", "Content", c => c.String());
            CreateIndex("dbo.BlogItems", "Author_Id");
            AddForeignKey("dbo.BlogItems", "Author_Id", "dbo.ApplicationUsers", "Id");
        }
    }
}

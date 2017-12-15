namespace JCarrollOnlineV2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addmigrationaddedblogitemidtoblogcommentitems : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.BlogItemComment", "BlogItem_Id", "dbo.BlogItem");
            DropIndex("dbo.BlogItemComment", new[] { "BlogItem_Id" });
            RenameColumn(table: "dbo.BlogItemComment", name: "BlogItem_Id", newName: "BlogItemId");
            AlterColumn("dbo.BlogItemComment", "BlogItemId", c => c.Int(nullable: false));
            CreateIndex("dbo.BlogItemComment", "BlogItemId");
            AddForeignKey("dbo.BlogItemComment", "BlogItemId", "dbo.BlogItem", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BlogItemComment", "BlogItemId", "dbo.BlogItem");
            DropIndex("dbo.BlogItemComment", new[] { "BlogItemId" });
            AlterColumn("dbo.BlogItemComment", "BlogItemId", c => c.Int());
            RenameColumn(table: "dbo.BlogItemComment", name: "BlogItemId", newName: "BlogItem_Id");
            CreateIndex("dbo.BlogItemComment", "BlogItem_Id");
            AddForeignKey("dbo.BlogItemComment", "BlogItem_Id", "dbo.BlogItem", "Id");
        }
    }
}

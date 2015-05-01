namespace JCarrollOnlineV2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedBlogComments : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Blogs", newName: "BlogItems");
            CreateTable(
                "dbo.BlogItemComments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Author = c.String(),
                        CreatedAt = c.DateTime(nullable: false),
                        Content = c.String(),
                        BlogItem_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BlogItems", t => t.BlogItem_Id)
                .Index(t => t.BlogItem_Id);
            
            DropColumn("dbo.BlogItems", "Title");
        }
        
        public override void Down()
        {
            AddColumn("dbo.BlogItems", "Title", c => c.String());
            DropForeignKey("dbo.BlogItemComments", "BlogItem_Id", "dbo.BlogItems");
            DropIndex("dbo.BlogItemComments", new[] { "BlogItem_Id" });
            DropTable("dbo.BlogItemComments");
            RenameTable(name: "dbo.BlogItems", newName: "Blogs");
        }
    }
}

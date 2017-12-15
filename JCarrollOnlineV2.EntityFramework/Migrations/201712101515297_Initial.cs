namespace JCarrollOnlineV2.EntityFramework.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            this.Sql(Properties.Sql.dbo_WriteLog);
            CreateTable(
                "dbo.ApplicationUser",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        MicroPostEmailNotifications = c.Boolean(nullable: false),
                        MicroPostSMSNotifications = c.Boolean(nullable: false),
                        Email = c.String(),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.IdentityUserClaim",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                        ApplicationUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ApplicationUser", t => t.ApplicationUser_Id)
                .Index(t => t.ApplicationUser_Id);
            
            CreateTable(
                "dbo.ForumThreadEntry",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(maxLength: 255),
                        Content = c.String(),
                        Locked = c.Boolean(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                        PostNumber = c.Int(nullable: false),
                        ParentId = c.Int(),
                        RootId = c.Int(),
                        Author_Id = c.String(nullable: false, maxLength: 128),
                        Forum_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ApplicationUser", t => t.Author_Id, cascadeDelete: true)
                .ForeignKey("dbo.Forum", t => t.Forum_Id, cascadeDelete: true)
                .Index(t => t.Author_Id)
                .Index(t => t.Forum_Id);
            
            CreateTable(
                "dbo.Forum",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 255),
                        Description = c.String(),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ForumModerator",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ForumId = c.Int(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Forum", t => t.ForumId, cascadeDelete: true)
                .Index(t => t.ForumId);
            
            CreateTable(
                "dbo.IdentityUserLogin",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        LoginProvider = c.String(),
                        ProviderKey = c.String(),
                        ApplicationUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.ApplicationUser", t => t.ApplicationUser_Id)
                .Index(t => t.ApplicationUser_Id);
            
            CreateTable(
                "dbo.MicroPost",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Content = c.String(maxLength: 140),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                        Author_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ApplicationUser", t => t.Author_Id)
                .Index(t => t.Author_Id);
            
            CreateTable(
                "dbo.IdentityUserRole",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(),
                        ApplicationUser_Id = c.String(maxLength: 128),
                        IdentityRole_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.ApplicationUser", t => t.ApplicationUser_Id)
                .ForeignKey("dbo.IdentityRole", t => t.IdentityRole_Id)
                .Index(t => t.ApplicationUser_Id)
                .Index(t => t.IdentityRole_Id);
            
            CreateTable(
                "dbo.BlogItem",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Content = c.String(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                        Author_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ApplicationUser", t => t.Author_Id, cascadeDelete: true)
                .Index(t => t.Author_Id);
            
            CreateTable(
                "dbo.BlogItemComment",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Author = c.String(),
                        CreatedAt = c.DateTime(nullable: false),
                        Content = c.String(),
                        BlogItem_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BlogItem", t => t.BlogItem_Id)
                .Index(t => t.BlogItem_Id);
            
            CreateTable(
                "dbo.IdentityRole",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.NLog",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MachineName = c.String(maxLength: 200),
                        SiteName = c.String(nullable: false, maxLength: 200),
                        Logged = c.DateTime(nullable: false),
                        Level = c.String(nullable: false, maxLength: 5),
                        UserName = c.String(maxLength: 200),
                        Message = c.String(nullable: false),
                        Logger = c.String(maxLength: 300),
                        Properties = c.String(),
                        ServerName = c.String(maxLength: 200),
                        Port = c.String(maxLength: 100),
                        Url = c.String(maxLength: 2000),
                        Https = c.Byte(nullable: false),
                        ServerAddress = c.String(maxLength: 100),
                        RemoteAddress = c.String(maxLength: 100),
                        Callsite = c.String(maxLength: 300),
                        Exception = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ApplicationUserApplicationUser",
                c => new
                    {
                        ApplicationUser_Id = c.String(nullable: false, maxLength: 128),
                        ApplicationUser_Id1 = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.ApplicationUser_Id, t.ApplicationUser_Id1 })
                .ForeignKey("dbo.ApplicationUser", t => t.ApplicationUser_Id)
                .ForeignKey("dbo.ApplicationUser", t => t.ApplicationUser_Id1)
                .Index(t => t.ApplicationUser_Id)
                .Index(t => t.ApplicationUser_Id1);
            
        }
        
        public override void Down()
        {
            this.Sql(Properties.Sql.dbo_DropWriteLog);
            DropForeignKey("dbo.IdentityUserRole", "IdentityRole_Id", "dbo.IdentityRole");
            DropForeignKey("dbo.BlogItemComment", "BlogItem_Id", "dbo.BlogItem");
            DropForeignKey("dbo.BlogItem", "Author_Id", "dbo.ApplicationUser");
            DropForeignKey("dbo.IdentityUserRole", "ApplicationUser_Id", "dbo.ApplicationUser");
            DropForeignKey("dbo.MicroPost", "Author_Id", "dbo.ApplicationUser");
            DropForeignKey("dbo.IdentityUserLogin", "ApplicationUser_Id", "dbo.ApplicationUser");
            DropForeignKey("dbo.ForumThreadEntry", "Forum_Id", "dbo.Forum");
            DropForeignKey("dbo.ForumModerator", "ForumId", "dbo.Forum");
            DropForeignKey("dbo.ForumThreadEntry", "Author_Id", "dbo.ApplicationUser");
            DropForeignKey("dbo.ApplicationUserApplicationUser", "ApplicationUser_Id1", "dbo.ApplicationUser");
            DropForeignKey("dbo.ApplicationUserApplicationUser", "ApplicationUser_Id", "dbo.ApplicationUser");
            DropForeignKey("dbo.IdentityUserClaim", "ApplicationUser_Id", "dbo.ApplicationUser");
            DropIndex("dbo.ApplicationUserApplicationUser", new[] { "ApplicationUser_Id1" });
            DropIndex("dbo.ApplicationUserApplicationUser", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.BlogItemComment", new[] { "BlogItem_Id" });
            DropIndex("dbo.BlogItem", new[] { "Author_Id" });
            DropIndex("dbo.IdentityUserRole", new[] { "IdentityRole_Id" });
            DropIndex("dbo.IdentityUserRole", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.MicroPost", new[] { "Author_Id" });
            DropIndex("dbo.IdentityUserLogin", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.ForumModerator", new[] { "ForumId" });
            DropIndex("dbo.ForumThreadEntry", new[] { "Forum_Id" });
            DropIndex("dbo.ForumThreadEntry", new[] { "Author_Id" });
            DropIndex("dbo.IdentityUserClaim", new[] { "ApplicationUser_Id" });
            DropTable("dbo.ApplicationUserApplicationUser");
            DropTable("dbo.NLog");
            DropTable("dbo.IdentityRole");
            DropTable("dbo.BlogItemComment");
            DropTable("dbo.BlogItem");
            DropTable("dbo.IdentityUserRole");
            DropTable("dbo.MicroPost");
            DropTable("dbo.IdentityUserLogin");
            DropTable("dbo.ForumModerator");
            DropTable("dbo.Forum");
            DropTable("dbo.ForumThreadEntry");
            DropTable("dbo.IdentityUserClaim");
            DropTable("dbo.ApplicationUser");
        }
    }
}

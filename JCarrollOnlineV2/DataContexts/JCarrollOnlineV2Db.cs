using JCarrollOnlineV2.Entities;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Web.Management;

namespace JCarrollOnlineV2.DataContexts
{
    public class LogEvent : WebRequestErrorEvent
    {
        public LogEvent(string message)
            : base(null, null, 100001, new Exception(message))
        {
        }
    }

    public class JCarrollOnlineV2Db : DbContext, IContext
    {
        public JCarrollOnlineV2Db()
            : base("JCarrollOnlineV2")
        {
            //Database.Log = Console.WriteLine;
            //LogEvent logEvent = new LogEvent("using {%0} as dbcontext" + "JCarrollOnlineV2");
            //Database.Log = s => { System.Diagnostics.Debug.Write(s); };
        }

        public static JCarrollOnlineV2Db Create()
        {
            return new JCarrollOnlineV2Db();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //#if !DEBUG
            //Database.SetInitializer(new MigrateDatabaseToLatestVersion<JCarrollOnlineV2Db, Configuration>()); 
            //#endif

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<IdentityUserLogin>()
                .HasKey(p => p.UserId);

            modelBuilder.Entity<IdentityUserRole>()
                .HasKey(p => p.UserId);

            modelBuilder.Entity<MicroPost>()
                .ToTable("MicroPost")
                .HasRequired(m => m.Author)
                .WithMany(m => m.MicroPosts)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<BlogItem>()
                .ToTable("BlogItem");

            modelBuilder.Entity<Forum>()
                .ToTable("Forum");

            modelBuilder.Entity<ForumModerator>()
                .ToTable("ForumModerator");

            modelBuilder.Entity<ForumThreadEntry>()
                .ToTable("ForumThreadEntry");

            //modelBuilder.Entity<ForumThreadEntry>()
            //    .HasRequired<ApplicationUser>(s => s.Author)
            //    .WithMany(t => t.ForumThreadEntries).HasForeignKey(m => m.AuthorId)
            //.WillCascadeOnDelete(false);

        }
        public virtual DbSet<IdentityRole> Roles { get; set; }
        public virtual DbSet<ApplicationUser> Users { get; set; }
        public virtual DbSet<IdentityUserClaim> UserClaims { get; set; }
        public virtual DbSet<IdentityUserLogin> UserLogins { get; set; }
        public virtual DbSet<IdentityUserRole> UserRoles { get; set; }

        public DbSet<Forum> Forums { get; set; }
        public DbSet<ForumModerator> ForumModerators { get; set; }
        public DbSet<ForumThreadEntry> ForumThreadEntries { get; set; }
        public DbSet<MicroPost> MicroPosts { get; set; }
        public DbSet<BlogItem> BlogItem { get; set; }
        public DbSet<BlogItemComment> BlogItemComments { get; set; }
    }
}
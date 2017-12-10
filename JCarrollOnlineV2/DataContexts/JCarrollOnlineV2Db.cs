using JCarrollOnlineV2.Entities;
using JCarrollOnlineV2.Migrations;
using Microsoft.AspNet.Identity.EntityFramework;
using NLog;
using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Globalization;
using System.Web.Management;

namespace JCarrollOnlineV2.DataContexts
{
    public class JCarrollOnlineV2Db : DbContext, IContext
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public JCarrollOnlineV2Db()
            : base("JCarrollOnlineV2")
        {
            //Database.Log = Console.WriteLine;
            //LogEvent logEvent = new LogEvent("using {%0} as dbcontext" + "JCarrollOnlineV2");
            //Database.Log = s => { System.Diagnostics.Debug.Write(s); };
        }

        public static JCarrollOnlineV2Db Create()
        {
            logger.Info(string.Format(CultureInfo.InvariantCulture, "Creating new db context, call stack: {0}", new System.Diagnostics.StackTrace()));
            return new JCarrollOnlineV2Db();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<JCarrollOnlineV2Db, Configuration>());
            base.OnModelCreating(modelBuilder);

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<IdentityUserLogin>()
                .HasKey(p => p.UserId)
                .ToTable("IdentityUserLogin");

            modelBuilder.Entity<IdentityUserRole>()
                .HasKey(p => p.UserId)
                .ToTable("IdentityUserRole");

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

            modelBuilder.Entity<Entities.NLog>()
                .HasKey(k => k.Id)
                .ToTable("NLog");

            //modelBuilder.Entity<ForumThreadEntry>()
            //    .HasRequired<ApplicationUser>(s => s.Author)
            //    .WithMany(t => t.ForumThreadEntries).HasForeignKey(m => m.AuthorId)
            //.WillCascadeOnDelete(false);

        }
        public virtual DbSet<IdentityRole> IdentityRole { get; set; }
        public virtual DbSet<ApplicationUser> ApplicationUser { get; set; }
        public virtual DbSet<IdentityUserClaim> IdentityUserClaim { get; set; }
        public virtual DbSet<IdentityUserLogin> IdentityUserLogin { get; set; }
        public virtual DbSet<IdentityUserRole> IdentityUserRole { get; set; }

        public DbSet<Forum> Forum { get; set; }
        public DbSet<ForumModerator> ForumModerator { get; set; }
        public DbSet<ForumThreadEntry> ForumThreadEntry { get; set; }
        public DbSet<MicroPost> MicroPost { get; set; }
        public DbSet<BlogItem> BlogItem { get; set; }
        public DbSet<BlogItemComment> BlogItemComment { get; set; }
        public DbSet<Entities.NLog> NLog { get; set; }
    }
}
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
    public class JCarrollOnlineV2Connection : DbContext, IJCarrollOnlineV2Context
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public JCarrollOnlineV2Connection()
            : base("JCarrollOnlineV2Connection")
        {
            //Database.Log = Console.WriteLine;
            //LogEvent logEvent = new LogEvent("using {%0} as dbcontext" + "JCarrollOnlineV2");
            //Database.Log = s => { logger.Info(s); };
        }

        public static JCarrollOnlineV2Connection Create()
        {
            var context = new JCarrollOnlineV2Connection();
            logger.Info(string.Format(CultureInfo.InvariantCulture, "Creating new db context, call stack: {0}", new System.Diagnostics.StackTrace()));
            logger.Info(string.Format(CultureInfo.InvariantCulture, "Using connection string: {0}", context.Database.Connection.ConnectionString));
            return context;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            if (modelBuilder == null)
            {
                throw new ArgumentNullException(nameof(modelBuilder));
            }

            Database.SetInitializer(new MigrateDatabaseToLatestVersion<JCarrollOnlineV2Connection, Configuration>());
            base.OnModelCreating(modelBuilder);

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<IdentityUserLogin>()
                .HasKey(p => p.UserId)
                .ToTable("IdentityUserLogin");

            modelBuilder.Entity<IdentityUserRole>()
                .HasKey(p => p.UserId)
                .ToTable("IdentityUserRole");

            modelBuilder.Entity<ApplicationUser>()
                .HasKey(m => m.Id);

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

            modelBuilder.Entity<ThreadEntry>()
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
        public DbSet<ThreadEntry> ForumThreadEntry { get; set; }
        public DbSet<MicroPost> MicroPost { get; set; }
        public DbSet<BlogItem> BlogItem { get; set; }
        public DbSet<BlogItemComment> BlogItemComment { get; set; }
        public DbSet<Entities.NLog> NLog { get; set; }
    }
}
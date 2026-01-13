using JCarrollOnlineV2.Entities;
using JCarrollOnlineV2.EntityFramework.Migrations;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace JCarrollOnlineV2.EntityFramework
{
    public class JCarrollOnlineV2DbContext : DbContext
    {
        //private static Logger logger = LogManager.GetCurrentClassLogger();

        public JCarrollOnlineV2DbContext()
#if DEBUG
        : base("JCarrollOnlineV2Connection")  // Use DEV connection
#else
        : base("JCarrollOnlineV2ProductionConnection")  // Use PROD connection
#endif      
        {
#if DEBUG
            Database.Log = sql => System.Diagnostics.Debug.WriteLine(sql);
#endif
        }


        public static JCarrollOnlineV2DbContext Create()
        {
            JCarrollOnlineV2DbContext context = new JCarrollOnlineV2DbContext();
            return context;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            if (modelBuilder == null)
            {
                throw new ArgumentNullException(nameof(modelBuilder));
            }

#if DEBUG
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<JCarrollOnlineV2DbContext, Configuration>());
#else
            Database.SetInitializer<JCarrollOnlineV2DbContext>(null); // No automatic migrations in production
#endif
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

        public virtual DbSet<Forum> Forum { get; set; }
        public virtual DbSet<ForumModerator> ForumModerator { get; set; }
        public virtual DbSet<ThreadEntry> ForumThreadEntry { get; set; }
        public virtual DbSet<MicroPost> MicroPost { get; set; }
        public virtual DbSet<BlogItem> BlogItem { get; set; }
        public virtual DbSet<BlogItemComment> BlogItemComment { get; set; }
        public virtual DbSet<Entities.NLog> NLog { get; set; }
        public virtual DbSet<ChatMessage> ChatMessages { get; set; }
    }
}

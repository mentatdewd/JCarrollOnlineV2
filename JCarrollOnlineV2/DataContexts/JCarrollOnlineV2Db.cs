using JCarrollOnlineV2.Entities;
using JCarrollOnlineV2.Migrations;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace JCarrollOnlineV2.DataContexts
{
    public class JCarrollOnlineV2Db : DbContext, IContext
    {
        public JCarrollOnlineV2Db()
            : base("JCarrollOnlineV2Connection")
        {
            //Database.Log = Console.WriteLine;
            Database.Log = s => { System.Diagnostics.Debug.Write(s); };
        }
        public static JCarrollOnlineV2Db Create()
        {
            return new JCarrollOnlineV2Db();
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
#if !DEBUG
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<JCarrollOnlineV2Db, Configuration>()); 
#endif

            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<IdentityUserLogin>().HasKey<string>(l => l.UserId);
            modelBuilder.Entity<IdentityRole>().HasKey<string>(r => r.Id);
            modelBuilder.Entity<IdentityUserRole>().HasKey(r => new { r.RoleId, r.UserId });

            modelBuilder.Entity<Micropost>()
                .HasRequired(m => m.Author)
                .WithMany(m => m.Microposts)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(m => m.Followers)
                .WithMany(m => m.Following)
                .Map(x => x.MapLeftKey("UserId")
                .MapRightKey("FollowerId")
                .ToTable("UserFollowers"));


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
        public DbSet<Micropost> Microposts { get; set; }
    }
}
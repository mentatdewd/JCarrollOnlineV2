using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JCarrollOnlineV2.Entities;
using Microsoft.AspNet.Identity.EntityFramework;
using JCarrollOnlineV2.DataContexts;
using System.Data.Entity.Infrastructure;

namespace JCarrollOnlineV2.Security
{
    public interface IJCarrollOnlineV2IdentityContext : IDisposable
    {
        DbSet<IdentityRole> Roles { get; set; }
        DbSet<ApplicationUser> Users { get; set; }
        DbSet<IdentityUserClaim> UserClaims { get; set; }
        DbSet<IdentityUserLogin> UserLogins { get; set; }
        DbSet<IdentityUserRole> UserRoles { get; set; }

        int SaveChanges();
        Task<int> SaveChangesAsync();
        DbEntityEntry Entry(object entity);
    }

    public class JCarrollOnlineV2IdentityDb : DbContext, IJCarrollOnlineV2IdentityContext
    {
        private static string _connectionString { get; set; }

        public JCarrollOnlineV2IdentityDb(string connectionString)
            : base(connectionString)
        {
            //Database.Log = Console.WriteLine;
            _connectionString = connectionString;
            Database.Log = s => { System.Diagnostics.Debug.Write(s); };
        }
        public static JCarrollOnlineV2IdentityDb Create()
        {
            return new JCarrollOnlineV2IdentityDb(_connectionString);
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
        }
        public virtual DbSet<IdentityRole> Roles { get; set; }
        public virtual DbSet<ApplicationUser> Users { get; set; }
        public virtual DbSet<IdentityUserClaim> UserClaims { get; set; }
        public virtual DbSet<IdentityUserLogin> UserLogins { get; set; }
        public virtual DbSet<IdentityUserRole> UserRoles { get; set; }
    }
}

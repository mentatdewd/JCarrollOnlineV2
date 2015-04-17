using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using JCarrollOnlineV2.Entities;
using Microsoft.AspNet.Identity.EntityFramework;
using JCarrollOnlineV2.ViewModels;
using JCarrollOnlineV2.Migrations;

namespace JCarrollOnlineV2.DataContexts
{
    public class JCarrollOnlineV2Db : DbContext
    {
        public JCarrollOnlineV2Db()
            : base("DefaultConnection")
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
//#if !DEBUG
//            Database.SetInitializer(new MigrateDatabaseToLatestVersion<JCarrollOnlineV2Db, Configuration>()); 
//#endif

            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<IdentityUserLogin>().HasKey<string>(l => l.UserId);
            modelBuilder.Entity<IdentityRole>().HasKey<string>(r => r.Id);
            modelBuilder.Entity<IdentityUserRole>().HasKey(r => new { r.RoleId, r.UserId });
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

        public DbSet<Relationship> Relationships { get; set; }
    }
}
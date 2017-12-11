using JCarrollOnlineV2.Entities;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Threading.Tasks;

namespace JCarrollOnlineV2.DataContexts
{
    public interface IJCarrollOnlineV2Context
    {
        DbSet<IdentityRole> IdentityRole { get; set; }
        DbSet<ApplicationUser> ApplicationUser { get; set; }
        DbSet<IdentityUserClaim> IdentityUserClaim { get; set; }
        DbSet<IdentityUserLogin> IdentityUserLogin { get; set; }
        DbSet<IdentityUserRole> IdentityUserRole { get; set; }

        DbSet<Forum> Forum { get; set; }

        DbSet<ForumModerator> ForumModerator { get; set; }

        DbSet<ThreadEntry> ForumThreadEntry { get; set; }

        DbSet<MicroPost> MicroPost { get; set; }

        DbSet<BlogItem> BlogItem { get; set; }
        DbSet<BlogItemComment> BlogItemComment { get; set; }
        DbSet<NLog> NLog { get; set; }

        int SaveChanges();
        Task<int> SaveChangesAsync();
        DbEntityEntry Entry(object entity);
    }
}

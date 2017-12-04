using JCarrollOnlineV2.Entities;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Threading.Tasks;

namespace JCarrollOnlineV2.DataContexts
{
    public interface IContext
    {
        DbSet<IdentityRole> Roles { get; set; }
        DbSet<ApplicationUser> Users { get; set; }
        DbSet<IdentityUserClaim> UserClaims { get; set; }
        DbSet<IdentityUserLogin> UserLogins { get; set; }
        DbSet<IdentityUserRole> UserRoles { get; set; }

        DbSet<Forum> Forums { get; set; }

        DbSet<ForumModerator> ForumModerators { get; set; }

        DbSet<ForumThreadEntry> ForumThreadEntries { get; set; }

        DbSet<MicroPost> MicroPosts { get; set; }

        DbSet<BlogItem> BlogItems { get; set; }
        DbSet<BlogItemComment> BlogItemComments { get; set; }

        int SaveChanges();
        Task<int> SaveChangesAsync();
        DbEntityEntry Entry(object entity);
    }
}

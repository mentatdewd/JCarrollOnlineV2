using JCarrollOnlineV2.Entities;
using JCarrollOnlineV2.ViewModels;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCarrollOnlineV2.DataContexts
{
    public interface IContext : IDisposable
    {
        DbSet<IdentityRole> Roles { get; set; }
        DbSet<ApplicationUser> Users { get; set; }
        DbSet<IdentityUserClaim> UserClaims { get; set; }
        DbSet<IdentityUserLogin> UserLogins { get; set; }
        DbSet<IdentityUserRole> UserRoles { get; set; }

        DbSet<Forum> Forums { get; set; }

        DbSet<ForumModerator> ForumModerators { get; set; }

        DbSet<ForumThreadEntry> ForumThreadEntries { get; set; }

        DbSet<Micropost> Microposts { get; set; }

        DbSet<Relationship> Relationships { get; set; }

        int SaveChanges();
        Task<int> SaveChangesAsync();
        DbEntityEntry Entry(object entity);
    }
}

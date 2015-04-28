using JCarrollOnlineV2.DataContexts;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace JCarrollOnlineV2.Entities
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
        // Navigation Property
        public virtual ICollection<ForumThreadEntry> ForumThreadEntries { get; set; }

        public virtual ICollection<Micropost> Microposts { get; set; }

        public virtual ICollection<ApplicationUser> Following { get; set; }
        public virtual ICollection<ApplicationUser> Followers { get; set; }

        public virtual ICollection<Blog> BlogItems { get; set; }
    }

    //public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IContext
    //{
    //    public ApplicationDbContext(string nameOrConnectionString)
    //        : base(nameOrConnectionString)
    //    {
    //    }
        
    //    public ApplicationDbContext()
    //        : base("JCarrollOnlineV2Connection", throwIfV1Schema: false)
    //    {
    //    }

    //    public static ApplicationDbContext Create()
    //    {
    //        return new ApplicationDbContext();
    //    }
    //}
}
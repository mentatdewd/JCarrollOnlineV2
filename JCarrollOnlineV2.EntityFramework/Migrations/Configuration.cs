namespace JCarrollOnlineV2.EntityFramework.Migrations
{
    using JCarrollOnlineV2.Entities;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<JCarrollOnlineV2DbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(JCarrollOnlineV2DbContext context)
        {
            AddAdminRoleAndUser(context);
        }

        const string _adminRole = "Administrator";
        const string _adminName = "administrator";

        private static bool AddAdminRoleAndUser(JCarrollOnlineV2DbContext context)
        {
            using(UserStore<ApplicationUser> userStore = new UserStore<ApplicationUser>(context))
            using (UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(userStore))
            using (RoleStore<IdentityRole> roleStore = new RoleStore<IdentityRole>(context))
            using (RoleManager<IdentityRole> roleManager = new RoleManager<IdentityRole>(roleStore))
            {

                if (!context.ApplicationUser.Any(u => u.UserName == _adminName))
                {
                    ApplicationUser adminUser = new ApplicationUser
                    {
                        UserName = _adminName,
                        Email = "mentatdewd@comcast.net",
                        EmailConfirmed = true
                    };

                    string password = "Admin@2024";

                    userManager.Create(adminUser, password);

                    if (!roleManager.RoleExists(_adminRole))
                    {
                        IdentityRole adminIdentityRole = new IdentityRole(_adminRole);
                        roleManager.Create(adminIdentityRole);
                    }

                    userManager.AddToRole(adminUser.Id, _adminRole);
                }
            }

            return true;
        }
    }
}

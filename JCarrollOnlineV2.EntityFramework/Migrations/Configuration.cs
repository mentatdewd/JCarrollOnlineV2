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
            //Deletes all data, from all tables, except for __MigrationHistory
            //context.Database.ExecuteSqlCommand("sp_MSForEachTable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL'");
            //context.Database.ExecuteSqlCommand("sp_MSForEachTable 'IF OBJECT_ID(''?'') NOT IN (ISNULL(OBJECT_ID(''[dbo].[__MigrationHistory]''),0)) DELETE FROM ?'");
            //context.Database.ExecuteSqlCommand("EXEC sp_MSForEachTable 'ALTER TABLE ? CHECK CONSTRAINT ALL'");
            //context.Database.ExecuteSqlCommand("DBCC CHECKIDENT (Fora, RESEED, 0)");
            //context.Database.ExecuteSqlCommand("DBCC CHECKIDENT (ForumThreadEntries, RESEED, 0)");

            //context.Database.ExecuteSqlCommand(@"TRUNCATE TABLE dbo.NLog");
            //AddAdminRoleAndUser(context);
        }

        const string _adminRole = "Administrator";
        const string _adminName = "administrator";

#pragma warning disable IDE0051 // Remove unused private members
        private static bool AddAdminRoleAndUser(JCarrollOnlineV2DbContext context)
#pragma warning restore IDE0051 // Remove unused private members
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

                    string password = "password";

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

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
            AddAdminRoleAndUser(context);
        }

        const string adminRole = "Administrator";
        const string adminName = "administrator";

        private bool AddAdminRoleAndUser(JCarrollOnlineV2DbContext context)
        {
            using(var userStore = new UserStore<ApplicationUser>(context))
            using (var userManager = new UserManager<ApplicationUser>(userStore))
            using (var roleStore = new RoleStore<IdentityRole>(context))
            using (var roleManager = new RoleManager<IdentityRole>(roleStore))
            {

                if (!context.ApplicationUser.Any(u => u.UserName == adminName))
                {
                    var adminUser = new ApplicationUser
                    {
                        UserName = adminName,
                        Email = "mentatdewd@comcast.net",
                        EmailConfirmed = true
                    };

                    string password = "password";

                    userManager.Create(adminUser, password);

                    if (!roleManager.RoleExists(adminRole))
                    {
                        IdentityRole adminIdentityRole = new IdentityRole(adminRole);
                        roleManager.Create(adminIdentityRole);
                    }

                    userManager.AddToRole(adminUser.Id, adminRole);
                }
            }

            return true;
        }
    }
}

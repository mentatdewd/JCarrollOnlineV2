namespace JCarrollOnlineV2.Migrations
{
    using JCarrollOnlineV2.DataContexts;
    using JCarrollOnlineV2.Entities;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<JCarrollOnlineV2.DataContexts.JCarrollOnlineV2Connection>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(DataContexts.JCarrollOnlineV2Connection context)
        {
            //Deletes all data, from all tables, except for __MigrationHistory
            //context.Database.ExecuteSqlCommand("sp_MSForEachTable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL'");
            //context.Database.ExecuteSqlCommand("sp_MSForEachTable 'IF OBJECT_ID(''?'') NOT IN (ISNULL(OBJECT_ID(''[dbo].[__MigrationHistory]''),0)) DELETE FROM ?'");
            //context.Database.ExecuteSqlCommand("EXEC sp_MSForEachTable 'ALTER TABLE ? CHECK CONSTRAINT ALL'");
            //context.Database.ExecuteSqlCommand("DBCC CHECKIDENT (Fora, RESEED, 0)");
            //context.Database.ExecuteSqlCommand("DBCC CHECKIDENT (ForumThreadEntries, RESEED, 0)");

            context.Database.ExecuteSqlCommand(@"TRUNCATE TABLE dbo.NLog");
            AddAdminRoleAndUser(context);
        }

        const string adminRole = "Administrator";
        const string adminName = "administrator";

        private bool AddAdminRoleAndUser(JCarrollOnlineV2Connection context)
        {
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            var PasswordHash = new PasswordHasher();
            var RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            if (!context.ApplicationUser.Any(u => u.UserName == adminName))
            {
                var adminUser = new ApplicationUser
                {
                    UserName = adminName,
                    Email = "mentatdewd@comcast.net",
                    EmailConfirmed = true
                };

                string password = "password";
                var pwHash = PasswordHash.HashPassword(password);
                UserManager.Create(adminUser, password);


                if (!RoleManager.RoleExists(adminRole))
                {
                    IdentityRole adminIdentityRole = new IdentityRole(adminRole);
                    RoleManager.Create(adminIdentityRole);
                }

                UserManager.AddToRole(adminUser.Id, adminRole);
            }

            return true;
        }
    }
}

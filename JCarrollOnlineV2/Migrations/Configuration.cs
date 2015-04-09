namespace JCarrollOnlineV2.Migrations
{
    using JCarrollOnlineV2.DataContexts;
    using JCarrollOnlineV2.Entities;
    using JCarrollOnlineV2.ViewModels;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Threading.Tasks;

    internal sealed class NewUser
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }
    }

    internal sealed class Configuration : DbMigrationsConfiguration<JCarrollOnlineV2Db>
    {
        private string loremIpsum = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(JCarrollOnlineV2.DataContexts.JCarrollOnlineV2Db context)
        {
            // Deletes all data, from all tables, except for __MigrationHistory
            context.Database.ExecuteSqlCommand("sp_MSForEachTable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL'");
            context.Database.ExecuteSqlCommand("sp_MSForEachTable 'IF OBJECT_ID(''?'') NOT IN (ISNULL(OBJECT_ID(''[dbo].[__MigrationHistory]''),0)) DELETE FROM ?'");
            context.Database.ExecuteSqlCommand("EXEC sp_MSForEachTable 'ALTER TABLE ? CHECK CONSTRAINT ALL'");
            context.Database.ExecuteSqlCommand("DBCC CHECKIDENT (Fora, RESEED, 0)");
            context.Database.ExecuteSqlCommand("DBCC CHECKIDENT (ForumThreadEntries, RESEED, 0)");

            //System.Diagnostics.Debugger.Launch();
            Dictionary<string, string> userIds = new Dictionary<string, string>();

            for (int i = 0; i < 4; i++)
            {
                var newUser = new NewUser() { UserName = string.Format("User{0}", i.ToString()), Email = string.Format("User{0}@test.com", i.ToString()), Password = string.Format("Password{0}", i.ToString()), Roles = new List<string>() { "Admin" } };
                var userId = AddRoleAndUser(context, newUser);
                if (userId == null)
                {
                    Console.Write("Seeding failed for user: " + string.Format("User{0}", i.ToString()));
                }
                userIds.Add(newUser.UserName, userId);
            }

            context.Forums.AddOrUpdate(x => x.Id,
                new Forum
                {
                    Id = 1,
                    Title = "Forum 1",
                    Description = "Forum 1 Description",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    ForumThreads = new List<ForumThreadEntry> 
                    {
#region Forum 1 Forum Thread 1
                        
                        new ForumThreadEntry { ForumThreadEntryId = 1, Title = "First Original Thread Entry", CreatedAt=DateTime.Now, UpdatedAt=DateTime.Now, PostNumber = 1, Content=loremIpsum, AuthorId=userIds["User0"]},
                        new ForumThreadEntry { ForumThreadEntryId = 5, Title = "First Child of First Thread Original Entry", ParentId = 1, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, PostNumber= 3, Content=loremIpsum, AuthorId=userIds["User1"]},
                        new ForumThreadEntry { ForumThreadEntryId = 6, Title = "Second Child of First Thread Original Entry", ParentId = 1, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, PostNumber= 4, Content=loremIpsum, AuthorId=userIds["User2"]},
                        new ForumThreadEntry { ForumThreadEntryId = 7, Title = "Third Child of First Thread Original Entry", ParentId = 1, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, PostNumber= 5, Content=loremIpsum, AuthorId=userIds["User3"]},
                        new ForumThreadEntry { ForumThreadEntryId = 8, Title = "Forth Child of First Thread Original Entry", ParentId = 1, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, PostNumber= 6, Content=loremIpsum, AuthorId=userIds["User0"]},

                        new ForumThreadEntry { ForumThreadEntryId = 9, Title = "First Child of First Child of Original Thread Entry", ParentId = 5, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, PostNumber=2, Content=loremIpsum, AuthorId=userIds["User1"] },
                        new ForumThreadEntry { ForumThreadEntryId = 10, Title = "First Child of Second Child of Original Thread Entry", ParentId = 6, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, PostNumber=7, Content=loremIpsum, AuthorId=userIds["User2"] },
                        new ForumThreadEntry { ForumThreadEntryId = 11, Title = "First Child of Third Child of Original Thread Entry", ParentId = 7, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, PostNumber=8, Content=loremIpsum, AuthorId=userIds["User3"] },
                        new ForumThreadEntry { ForumThreadEntryId = 12, Title = "First Child of Forth Child or Original Thread Entry", ParentId = 8, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, PostNumber=9, Content=loremIpsum, AuthorId=userIds["User0"] },

                        new ForumThreadEntry { ForumThreadEntryId = 13, Title = "Second Child of First Child of Original Thread Entry", ParentId = 5, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, PostNumber=10, Content=loremIpsum, AuthorId=userIds["User1"] },
                        new ForumThreadEntry { ForumThreadEntryId = 14, Title = "Second Child of Second Child of Original Thread Entry", ParentId = 6, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, PostNumber=11, Content=loremIpsum, AuthorId=userIds["User2"]},
                        new ForumThreadEntry { ForumThreadEntryId = 15, Title = "Second Child of Third Child of Original Thread Entry", ParentId = 7, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, PostNumber=12, Content=loremIpsum, AuthorId=userIds["User3"] },
                        new ForumThreadEntry { ForumThreadEntryId = 16, Title = "Second Child of Forth Child or Original Thread Entry", ParentId = 8, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, PostNumber=13, Content=loremIpsum, AuthorId=userIds["User0"] },

                        new ForumThreadEntry { ForumThreadEntryId = 17, Title = "First Child of First Child of First Child of Original Thread Entry", ParentId = 9, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, PostNumber=14, Content=loremIpsum, AuthorId=userIds["User1"] },
                        new ForumThreadEntry { ForumThreadEntryId = 18, Title = "Second Child of First Child of First Child of Original Thread Entry", ParentId = 9, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, PostNumber=15, Content=loremIpsum, AuthorId=userIds["User2"] },
#endregion
#region Forum 1 Forum Thread 2
                        new ForumThreadEntry { ForumThreadEntryId = 19, Title = "Second Original Thread Entry", CreatedAt=DateTime.Now, UpdatedAt=DateTime.Now, PostNumber= 1, Content=loremIpsum, AuthorId=userIds["User3"] },
                        new ForumThreadEntry { ForumThreadEntryId = 20, Title = "First child of Second Original Thread Entry", ParentId = 19, CreatedAt=DateTime.Now, UpdatedAt=DateTime.Now, PostNumber=2, Content=loremIpsum, AuthorId=userIds["User0"]}
#endregion
                    }

                },
                new Forum
                {
                    Id = 2,
                    Title = "Forum 2",
                    Description = "Forum 2 Description",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    ForumThreads = new List<ForumThreadEntry>
                    {
                        new ForumThreadEntry { ForumThreadEntryId = 2, Title = "Second Original Thread Entry", CreatedAt=DateTime.Now, UpdatedAt=DateTime.Now, PostNumber=1, Content=loremIpsum, AuthorId=userIds["User1"]}
                    }
                },
                new Forum
                {
                    Id = 3,
                    Title = "Forum 3",
                    Description = "Forum 3 Description",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    ForumThreads = new List<ForumThreadEntry>
                    {
                        new ForumThreadEntry { ForumThreadEntryId = 3, Title = "Third Original Thread Entry", CreatedAt=DateTime.Now, UpdatedAt=DateTime.Now, PostNumber=1, Content=loremIpsum, AuthorId=userIds["User2"]}
                    }
                },
                new Forum
                {
                    Id = 4,
                    Title = "Forum 4",
                    Description = "Forum 4 Description",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    ForumThreads = new List<ForumThreadEntry>
                    {
                        new ForumThreadEntry { ForumThreadEntryId=4, Title="Forth Original Thread Entry", CreatedAt=DateTime.Now, UpdatedAt=DateTime.Now, PostNumber=1, Content=loremIpsum, AuthorId=userIds["User3"]}
                    }
                }
            );
        }
        string AddRoleAndUser(JCarrollOnlineV2Db context, NewUser newUser)
        {
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            var RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            IdentityResult result = IdentityResult.Success;

            var user = UserManager.FindByName(newUser.UserName);

            if (user == null)
            {
                user = new ApplicationUser() { UserName = newUser.UserName, Email = newUser.Email };
                result = UserManager.Create(user, newUser.Password);

                if (!result.Succeeded)
                {
                    Console.Write("Unable to create user: " + newUser.UserName);
                    return null;
                }
            }

            foreach (string r in newUser.Roles)
            {
                if (!RoleManager.RoleExists(r))
                {
                    var role = new IdentityRole(r);
                    result = RoleManager.Create(role);
                    if (!result.Succeeded)
                    {
                        Console.Write("Unable to create role: " + r);
                        return null;
                    }
                }

                result = UserManager.AddToRole(user.Id, r);
                if (!result.Succeeded)
                {
                    Console.Write("Unable to add user '" + newUser.UserName + "' to role '" + r + "'.");
                }
            }

            return UserManager.FindByName(newUser.UserName).Id;
        }
    }
}
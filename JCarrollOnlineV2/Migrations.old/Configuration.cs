using System;
using System.Linq;
using JCarrollOnlineV2.Entities;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using JCarrollOnlineV2.ViewModels;
using JCarrollOnlineV2.DataContexts;
using System.Data.Entity.Migrations;
using Microsoft.AspNet.Identity.EntityFramework;

namespace JCarrollOnlineV2.Migrations
{

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
            AutomaticMigrationDataLossAllowed = true;
            //CheckForPendingMigrations(this);
        }

        protected override void Seed(JCarrollOnlineV2.DataContexts.JCarrollOnlineV2Db context)
        {
            // Deletes all data, from all tables, except for __MigrationHistory
            //context.Database.ExecuteSqlCommand("sp_MSForEachTable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL'");
            //context.Database.ExecuteSqlCommand("sp_MSForEachTable 'IF OBJECT_ID(''?'') NOT IN (ISNULL(OBJECT_ID(''[dbo].[__MigrationHistory]''),0)) DELETE FROM ?'");
            //context.Database.ExecuteSqlCommand("EXEC sp_MSForEachTable 'ALTER TABLE ? CHECK CONSTRAINT ALL'");
            //context.Database.ExecuteSqlCommand("DBCC CHECKIDENT (Fora, RESEED, 0)");
            //context.Database.ExecuteSqlCommand("DBCC CHECKIDENT (ForumThreadEntries, RESEED, 0)");

            //System.Diagnostics.Debugger.Launch();
#if DEBUG
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
                System.Diagnostics.Trace.WriteLine(string.Format("Added userId: {0}", userId));
            }

            context.Forums.AddOrUpdate(x => x.Id,
                new Forum
                {
                    Id = 1,
                    Title = "Forum 1",
                    Description = "Forum 1 Description",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    ForumThreadEntries = new List<ForumThreadEntry> 
                    {
#region Forum 1 Forum Thread 1
                        
                        new ForumThreadEntry { ForumThreadEntryId = 1, RootId=1, Title = "ThreadId = 1, ParentId = null", CreatedAt=DateTime.Now, UpdatedAt=DateTime.Now, PostNumber = 1, Content=loremIpsum, AuthorId=userIds["User0"], ForumId=1},
                        new ForumThreadEntry { ForumThreadEntryId = 5, RootId=1, Title = "ThreadId = 5, ParentId = 1", ParentId = 1, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, PostNumber= 3, Content=loremIpsum, AuthorId=userIds["User1"], ForumId=1},
                        new ForumThreadEntry { ForumThreadEntryId = 6, RootId=1, Title = "ThreadId = 6, ParentId = 1", ParentId = 1, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, PostNumber= 4, Content=loremIpsum, AuthorId=userIds["User2"], ForumId=1},
                        new ForumThreadEntry { ForumThreadEntryId = 7, RootId=1, Title = "ThreadId = 7, ParentId = 1", ParentId = 1, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, PostNumber= 5, Content=loremIpsum, AuthorId=userIds["User3"], ForumId=1},
                        new ForumThreadEntry { ForumThreadEntryId = 8, RootId=1, Title = "ThreadId = 8, ParentId = 1", ParentId = 1, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, PostNumber= 6, Content=loremIpsum, AuthorId=userIds["User0"], ForumId=1},

                        new ForumThreadEntry { ForumThreadEntryId = 9, RootId=1, Title = "ThreadId = 9, ParentId = 5", ParentId = 5, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, PostNumber=2, Content=loremIpsum, AuthorId=userIds["User1"], ForumId=1 },
                        new ForumThreadEntry { ForumThreadEntryId = 10, RootId=1, Title = "ThreadId = 10, ParentId = 6", ParentId = 6, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, PostNumber=7, Content=loremIpsum, AuthorId=userIds["User2"], ForumId=1 },
                        new ForumThreadEntry { ForumThreadEntryId = 11, RootId=1, Title = "ThreadId = 11, ParentId = 7", ParentId = 7, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, PostNumber=8, Content=loremIpsum, AuthorId=userIds["User3"], ForumId=1 },
                        new ForumThreadEntry { ForumThreadEntryId = 12, RootId=1, Title = "ThreadId = 12, ParentId = 8", ParentId = 8, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, PostNumber=9, Content=loremIpsum, AuthorId=userIds["User0"], ForumId=1 },

                        new ForumThreadEntry { ForumThreadEntryId = 13, RootId=1, Title = "ThreadId = 13, ParentId = 5", ParentId = 5, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, PostNumber=10, Content=loremIpsum, AuthorId=userIds["User1"], ForumId=1 },
                        new ForumThreadEntry { ForumThreadEntryId = 14, RootId=1, Title = "ThreadId = 14, ParentId = 6", ParentId = 6, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, PostNumber=11, Content=loremIpsum, AuthorId=userIds["User2"], ForumId=1},
                        new ForumThreadEntry { ForumThreadEntryId = 15, RootId=1, Title = "ThreadId = 15, ParentId = 7", ParentId = 7, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, PostNumber=12, Content=loremIpsum, AuthorId=userIds["User3"], ForumId=1 },
                        new ForumThreadEntry { ForumThreadEntryId = 16, RootId=1, Title = "ThreadId = 16, ParentId = 8", ParentId = 8, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, PostNumber=13, Content=loremIpsum, AuthorId=userIds["User0"], ForumId=1 },

                        new ForumThreadEntry { ForumThreadEntryId = 17, RootId=1, Title = "ThreadId = 17, ParentId = 9", ParentId = 9, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, PostNumber=14, Content=loremIpsum, AuthorId=userIds["User1"], ForumId=1 },
                        new ForumThreadEntry { ForumThreadEntryId = 18, RootId=1, Title = "ThreadId = 18, ParentId = 9", ParentId = 9, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, PostNumber=15, Content=loremIpsum, AuthorId=userIds["User2"], ForumId=1 },
#endregion
#region Forum 1 Forum Thread 2
                        new ForumThreadEntry { ForumThreadEntryId = 19, RootId=19, Title = "Second Original Thread Entry", CreatedAt=DateTime.Now, UpdatedAt=DateTime.Now, PostNumber= 1, Content=loremIpsum, AuthorId=userIds["User3"], ForumId=1 },
                        new ForumThreadEntry { ForumThreadEntryId = 20, RootId=19, Title = "First child of Second Original Thread Entry", ParentId = 19, CreatedAt=DateTime.Now, UpdatedAt=DateTime.Now, PostNumber=2, Content=loremIpsum, AuthorId=userIds["User0"], ForumId=1}
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
                    ForumThreadEntries = new List<ForumThreadEntry>
                    {
                        new ForumThreadEntry { Id = 2, RootId=2, Title = "Second Original Thread Entry", CreatedAt=DateTime.Now, UpdatedAt=DateTime.Now, PostNumber=1, Content=loremIpsum, Author=userIds["User1"], ForumId=2}
                    }
                },
                new Forum
                {
                    ForumId = 3,
                    Title = "Forum 3",
                    Description = "Forum 3 Description",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    ForumThreads = new List<ForumThreadEntry>
                    {
                        new ForumThreadEntry { ForumThreadEntryId = 3, RootId=3, Title = "Third Original Thread Entry", CreatedAt=DateTime.Now, UpdatedAt=DateTime.Now, PostNumber=1, Content=loremIpsum, AuthorId=userIds["User2"], ForumId=3}
                    }
                },
                new Forum
                {
                    ForumId = 4,
                    Title = "Forum 4",
                    Description = "Forum 4 Description",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    ForumThreads = new List<ForumThreadEntry>
                    {
                        new ForumThreadEntry { ForumThreadEntryId=4, RootId=4, Title="Forth Original Thread Entry", CreatedAt=DateTime.Now, UpdatedAt=DateTime.Now, PostNumber=1, Content=loremIpsum, AuthorId=userIds["User3"], ForumId=4}
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
#endif
        }
    }
}
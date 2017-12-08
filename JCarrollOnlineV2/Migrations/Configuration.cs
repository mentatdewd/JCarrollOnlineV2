using System;
using System.Linq;
using JCarrollOnlineV2.Entities;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using JCarrollOnlineV2.ViewModels;
using JCarrollOnlineV2.DataContexts;
using System.Data.Entity.Migrations;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using System.Web.Security;

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
        }

        const string adminRole = "Administrator";
        IdentityRole identityAdminRole = new IdentityRole(adminRole);
        const string adminName = "administrator";

        private bool AddAdminRoleAndUser(JCarrollOnlineV2Db context)
        {
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            var PasswordHash = new PasswordHasher();
            var RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            if(!context.Users.Any(u => u.UserName == adminName))
            {
                var adminUser = new ApplicationUser
                {
                    UserName = adminName,
                    Email = "mentatdewd@comcast.net",
                    PasswordHash = PasswordHash.HashPassword("password"),
                    EmailConfirmed = true
                };

                UserManager.Create(adminUser);

                if (!RoleManager.RoleExists(adminRole))
                {
                    RoleManager.Create(identityAdminRole);
                }

                UserManager.AddToRole(adminUser.Id, adminRole);
            }

            return true;
        }

        protected override void Seed(JCarrollOnlineV2Db context)
        {
#if DEBUG
            // System.Diagnostics.Debugger.Launch();

            //Deletes all data, from all tables, except for __MigrationHistory
            context.Database.ExecuteSqlCommand("sp_MSForEachTable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL'");
            context.Database.ExecuteSqlCommand("sp_MSForEachTable 'IF OBJECT_ID(''?'') NOT IN (ISNULL(OBJECT_ID(''[dbo].[__MigrationHistory]''),0)) DELETE FROM ?'");
            context.Database.ExecuteSqlCommand("EXEC sp_MSForEachTable 'ALTER TABLE ? CHECK CONSTRAINT ALL'");
            context.Database.ExecuteSqlCommand("DBCC CHECKIDENT (Fora, RESEED, 0)");
            context.Database.ExecuteSqlCommand("DBCC CHECKIDENT (ForumThreadEntries, RESEED, 0)");
            //List<ApplicationUser> users = new List<ApplicationUser>();

            //for (int i = 0; i < 4; i++)
            //{
            //    var newUser = new NewUser() { UserName = string.Format("User{0}", i.ToString()), Email = string.Format("User{0}@test.com", i.ToString()), Password = string.Format("Password{0}", i.ToString()), Roles = new List<string>() { "Administrator" } };
            //    var user = AddRoleAndUser(context, newUser);
            //    if (user == null)
            //    {
            //        Console.Write("Seeding failed for user: " + string.Format("User{0}", i.ToString()));
            //    }
            //    users.Add(user);
            //    System.Diagnostics.Trace.WriteLine(string.Format("Added userId: {0}", newUser.UserName));
            //}

            AddAdminRoleAndUser(context);

//            context.Forums.AddOrUpdate(x => x.Id,
//                new Forum
//                {
//                    Id = 0,
//                    Title = "Forum 1",
//                    Description = "Forum 1 Description",
//                    CreatedAt = DateTime.Now,
//                    UpdatedAt = DateTime.Now,
//                    ForumThreadEntries = new List<ForumThreadEntry>
//                    {
//            #region Forum 1 Forum Thread 1
                        
//                        new ForumThreadEntry { Id = 0, RootId=0, Title = "ThreadId = 0, ParentId = null", CreatedAt=DateTime.Now, UpdatedAt=DateTime.Now, PostNumber = 1, Content=loremIpsum, Author=users[0]},
//                        new ForumThreadEntry { Id = 4, RootId=0, Title = "ThreadId = 4, ParentId = 0", ParentId = 0, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, PostNumber= 3, Content=loremIpsum, Author=users[1]},
//                        new ForumThreadEntry { Id = 5, RootId=0, Title = "ThreadId = 5, ParentId = 0", ParentId = 0, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, PostNumber= 4, Content=loremIpsum, Author=users[2]},
//                        new ForumThreadEntry { Id = 6, RootId=0, Title = "ThreadId = 6, ParentId = 0", ParentId = 0, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, PostNumber= 5, Content=loremIpsum, Author=users[3]},
//                        new ForumThreadEntry { Id = 7, RootId=0, Title = "ThreadId = 7, ParentId = 0", ParentId = 0, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, PostNumber= 6, Content=loremIpsum, Author=users[0]},

//                        new ForumThreadEntry { Id = 8, RootId=0, Title = "ThreadId = 8, ParentId = 4", ParentId = 4, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, PostNumber=2, Content=loremIpsum, Author=users[1]},
//                        new ForumThreadEntry { Id = 9, RootId=0, Title = "ThreadId = 9, ParentId = 5", ParentId = 5, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, PostNumber=7, Content=loremIpsum, Author=users[2]},
//                        new ForumThreadEntry { Id = 10, RootId=0, Title = "ThreadId = 10, ParentId = 6", ParentId = 6, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, PostNumber=8, Content=loremIpsum, Author=users[3]},
//                        new ForumThreadEntry { Id = 11, RootId=0, Title = "ThreadId = 11, ParentId = 7", ParentId = 7, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, PostNumber=9, Content=loremIpsum, Author=users[0]},

//                        new ForumThreadEntry { Id = 12, RootId=0, Title = "ThreadId = 12, ParentId = 4", ParentId = 4, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, PostNumber=10, Content=loremIpsum, Author=users[1]},
//                        new ForumThreadEntry { Id = 13, RootId=0, Title = "ThreadId = 13, ParentId = 5", ParentId = 5, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, PostNumber=11, Content=loremIpsum, Author=users[2]},
//                        new ForumThreadEntry { Id = 14, RootId=0, Title = "ThreadId = 14, ParentId = 6", ParentId = 6, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, PostNumber=12, Content=loremIpsum, Author=users[3]},
//                        new ForumThreadEntry { Id = 15, RootId=0, Title = "ThreadId = 15, ParentId = 7", ParentId = 7, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, PostNumber=13, Content=loremIpsum, Author=users[0]},

//                        new ForumThreadEntry { Id = 16, RootId=0, Title = "ThreadId = 16, ParentId = 8", ParentId = 8, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, PostNumber=14, Content=loremIpsum, Author=users[1]},
//                        new ForumThreadEntry { Id = 17, RootId=0, Title = "ThreadId = 17, ParentId = 8", ParentId = 8, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, PostNumber=15, Content=loremIpsum, Author=users[2]},
//#endregion
//            #region Forum 1 Forum Thread 2
//                        new ForumThreadEntry { Id = 18, RootId=18, Title = "Second Original Thread Entry", CreatedAt=DateTime.Now, UpdatedAt=DateTime.Now, PostNumber= 1, Content=loremIpsum, Author=users[3]},
//                        new ForumThreadEntry { Id = 19, RootId=18, Title = "First child of Second Original Thread Entry", ParentId = 18, CreatedAt=DateTime.Now, UpdatedAt=DateTime.Now, PostNumber=2, Content=loremIpsum, Author=users[0]}
//            #endregion
//                    }

//                },
//                new Forum
//                {
//                    Id = 1,
//                    Title = "Forum 2",
//                    Description = "Forum 2 Description",
//                    CreatedAt = DateTime.Now,
//                    UpdatedAt = DateTime.Now,
//                    ForumThreadEntries = new List<ForumThreadEntry>
//                    {
//                        new ForumThreadEntry { Id = 1, RootId=1, Title = "Second Original Thread Entry", CreatedAt=DateTime.Now, UpdatedAt=DateTime.Now, PostNumber=1, Content=loremIpsum, Author=users[1]}
//                    }
//                },
//                new Forum
//                {
//                    Id = 2,
//                    Title = "Forum 3",
//                    Description = "Forum 3 Description",
//                    CreatedAt = DateTime.Now,
//                    UpdatedAt = DateTime.Now,
//                    ForumThreadEntries = new List<ForumThreadEntry>
//                    {
//                        new ForumThreadEntry { Id = 2, RootId=2, Title = "Third Original Thread Entry", CreatedAt=DateTime.Now, UpdatedAt=DateTime.Now, PostNumber=1, Content=loremIpsum, Author=users[2]}
//                    }
//                },
//                new Forum
//                {
//                    Id = 3,
//                    Title = "Forum 4",
//                    Description = "Forum 4 Description",
//                    CreatedAt = DateTime.Now,
//                    UpdatedAt = DateTime.Now,
//                    ForumThreadEntries = new List<ForumThreadEntry>
//                    {
//                        new ForumThreadEntry { Id=3, RootId=3, Title="Forth Original Thread Entry", CreatedAt=DateTime.Now, UpdatedAt=DateTime.Now, PostNumber=1, Content=loremIpsum, Author=users[3]}
//                    }
//                }
//            );
//            context.SaveChanges();
        }
        ApplicationUser AddRoleAndUser(JCarrollOnlineV2Db context, NewUser newUser)
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

            return user;
#endif
        }
    }
}
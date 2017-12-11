﻿using JCarrollOnlineV2.DataContexts;
using JCarrollOnlineV2.Entities;
using JCarrollOnlineV2.ViewModels.Users;
using Microsoft.AspNet.Identity;
using NLog;
using Omu.ValueInjecter;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace JCarrollOnlineV2.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private IJCarrollOnlineV2Context _data { get; set; }

        public UsersController()
            : this(null)
        {

        }

        public UsersController(IJCarrollOnlineV2Context dataContext)
        {
            _data = dataContext ?? new JCarrollOnlineV2Connection();
        }

        // GET: Users
        public async Task<ActionResult> Index()
        {
            UsersIndexViewModel usersIndexViewModel = new UsersIndexViewModel();

            usersIndexViewModel.PageTitle = "Users";

            var users = await _data.ApplicationUser.Include("Following").Include("Followers").ToListAsync();

            foreach (var user in users)
            {
                UserItemViewModel userItemViewModel = new UserItemViewModel(logger);

                userItemViewModel.User.InjectFrom(user);
                userItemViewModel.MicroPostsAuthored = await _data.ApplicationUser.Include("MicroPosts").Where(u => u.Id == user.Id).Select(u => u.MicroPosts).CountAsync();
                usersIndexViewModel.Users.Add(userItemViewModel);
            }

            return View(usersIndexViewModel);
        }

        // GET: Users/Details/5
        public async Task<ActionResult> Details(string userId)
        {
            UserDetailViewModel userDetailViewModel = new UserDetailViewModel();

            string currentUserId = User.Identity.GetUserId();
            ApplicationUser currentUser = await _data.ApplicationUser.FirstOrDefaultAsync(x => x.Id == currentUserId);
            ApplicationUser user = await _data.ApplicationUser.Include("Following").Include("Followers").SingleAsync(m => m.Id == userId);

            userDetailViewModel.UserInfoViewModel.User.InjectFrom(user);

            //udVM.UserInfoVM.MicroPostEmailNotifications = user.MicroPostEmailNotifications;
            //udVM.UserInfoVM.MicroPostSMSNotifications = user.MicroPostSMSNotifications;
            userDetailViewModel.UserInfoViewModel.UserId = currentUserId;

            userDetailViewModel.UserStatsViewModel = new UserStatsViewModel();
            userDetailViewModel.UserStatsViewModel.UsersFollowing = new UserFollowingViewModel();
            userDetailViewModel.UserStatsViewModel.User.InjectFrom(user);

            foreach (var following in user.Following)
            {
                UserItemViewModel userItemViewModel = new UserItemViewModel(logger);

                userItemViewModel.User.InjectFrom(following);
                userItemViewModel.MicroPostsAuthored = await _data.MicroPost.Where(u => u.Author.Id == following.Id).CountAsync();
                userDetailViewModel.UserStatsViewModel.UsersFollowing.Users.Add(userItemViewModel);
            }

            userDetailViewModel.UserStatsViewModel.UserFollowers = new UserFollowersViewModel();

            foreach (var follower in user.Followers)
            {
                UserItemViewModel userItemViewModel = new UserItemViewModel(logger);

                userItemViewModel.User.InjectFrom(follower);
                userItemViewModel.MicroPostsAuthored = await _data.MicroPost.Where(u => u.Author.Id == follower.Id).CountAsync();
                userDetailViewModel.UserStatsViewModel.UserFollowers.Users.Add(userItemViewModel);
            }

            return View(userDetailViewModel);
        }

        public async Task<ActionResult> Following(string userId)
        {
            UserDetailViewModel userDetailViewModel = new UserDetailViewModel();

            userDetailViewModel.PageTitle = "Following";
            userDetailViewModel.UserInfoViewModel = new UserItemViewModel(logger);

            ApplicationUser user = await _data.ApplicationUser.Include("Following").Include("Followers").SingleAsync(m => m.Id == userId);

            userDetailViewModel.UserStatsViewModel = new UserStatsViewModel();
            userDetailViewModel.User.InjectFrom(user);
            userDetailViewModel.UserInfoViewModel.User.InjectFrom(user);
            userDetailViewModel.UserStatsViewModel.User.InjectFrom(user);

            foreach (var following in user.Following)
            {
                UserItemViewModel userItemViewModel = new UserItemViewModel(logger);
                userItemViewModel.User.InjectFrom(following);
                userItemViewModel.MicroPostsAuthored = following.MicroPosts.Count();
                userDetailViewModel.UserStatsViewModel.UsersFollowing.Users.Add(userItemViewModel);
            }

            foreach (var follower in user.Followers)
            {
                UserItemViewModel userItemViewModel = new UserItemViewModel(logger);
                userItemViewModel.InjectFrom(follower);
                userItemViewModel.MicroPostsAuthored = follower.MicroPosts.Count();
                userDetailViewModel.UserStatsViewModel.UserFollowers.Users.Add(userItemViewModel);
            }

            return View("Show_Follow", userDetailViewModel);
        }

        public async Task<ActionResult> Followed(string userId)
        {
            UserDetailViewModel userDetailViewModel = new UserDetailViewModel();

            userDetailViewModel.PageTitle = "Followers";
            userDetailViewModel.UserInfoViewModel = new UserItemViewModel(logger);

            ApplicationUser user = await _data.ApplicationUser.Include("Following").Include("Followers").SingleAsync(m => m.Id == userId);

            userDetailViewModel.UserStatsViewModel = new UserStatsViewModel();
            userDetailViewModel.User.InjectFrom(user);
            userDetailViewModel.UserInfoViewModel.User.InjectFrom(user);
            userDetailViewModel.UserStatsViewModel.User.InjectFrom(user);

            foreach (var following in user.Following)
            {
                UserItemViewModel userItemViewModel = new UserItemViewModel(logger);

                userItemViewModel.User.InjectFrom(following);
                userItemViewModel.MicroPostsAuthored = following.MicroPosts.Count();
                userDetailViewModel.UserStatsViewModel.UsersFollowing.Users.Add(userItemViewModel);
            }

            foreach (var follower in user.Followers)
            {
                UserItemViewModel userItemViewModel = new UserItemViewModel(logger);

                userItemViewModel.User.InjectFrom(follower);
                userItemViewModel.MicroPostsAuthored = follower.MicroPosts.Count();
                userDetailViewModel.UserStatsViewModel.UserFollowers.Users.Add(userItemViewModel);
            }

            return View("Show_Follow", userDetailViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Follow([Bind(Include = "UserId")]  UserItemViewModel followUser)
        {

            if (ModelState.IsValid)
            {
                string currentUserId = User.Identity.GetUserId();
                ApplicationUser currentUser = await _data.ApplicationUser.FirstOrDefaultAsync(x => x.Id == currentUserId);
                var followingUser = await _data.ApplicationUser.FirstOrDefaultAsync(x => x.Id == followUser.UserId);

                currentUser.Following.Add(followingUser);
                await _data.SaveChangesAsync();
            }

            return RedirectToAction("Details", new { userid = followUser.UserId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Unfollow([Bind(Include = "UserId")]  UserItemViewModel followUser)
        {

            if (ModelState.IsValid)
            {
                string currentUserId = User.Identity.GetUserId();
                ApplicationUser currentUser = await _data.ApplicationUser.FirstOrDefaultAsync(x => x.Id == currentUserId);
                var followingUser = await _data.ApplicationUser.FirstOrDefaultAsync(x => x.Id == followUser.UserId);

                currentUser.Following.Remove(followingUser);
                await _data.SaveChangesAsync();
            }

            return RedirectToAction("Details", new { userid = followUser.UserId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UserSettings([Bind(Include = "UserId,MicroPostEmailNotifications,MicroPostSMSNotifications")] UserItemViewModel userItemViewModel)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = await _data.ApplicationUser.FirstOrDefaultAsync(x => x.Id == userItemViewModel.UserId);

                //user.MicroPostEmailNotifications = auVM.MicroPostEmailNotifications;
                //user.MicroPostSMSNotifications = auVM.MicroPostSMSNotifications;
                await _data.SaveChangesAsync();
            }

            return RedirectToAction("Details", new { userid = userItemViewModel.UserId });
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        [HttpPost]
        public async Task<ActionResult> Create(FormCollection collection)
        {
            return await Task.Run<ActionResult>(() =>
            {
                try
                {
                    // TODO: Add insert logic here
                    return RedirectToAction("Index");
                }
                catch
                {
                    return View();
                }
            });
        }

        // GET: Users/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Users/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(int id, FormCollection collection)
        {
            return await Task.Run<ActionResult>(() =>
            {
                try
                {
                    // TODO: Add update logic here
                    return RedirectToAction("Index");
                }
                catch
                {
                    return View();
                }
            });
        }

        // GET: Users/Delete/5
        public async Task<ActionResult> Delete(string userId)
        {
            return await Task.Run<ActionResult>(() =>
            {
                return View();
            });
        }

        // POST: Users/Delete/5
        [HttpPost]
        public async Task<ActionResult> Delete(int id, FormCollection collection)
        {
            return await Task.Run<ActionResult>(() =>
            {
                try
                {
                    // TODO: Add delete logic here
                    return RedirectToAction("Index");
                }
                catch
                {
                    return View();
                }
            });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
            }

            base.Dispose(disposing);
        }
    }
}

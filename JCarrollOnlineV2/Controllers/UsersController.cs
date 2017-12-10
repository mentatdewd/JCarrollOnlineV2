using JCarrollOnlineV2.DataContexts;
using JCarrollOnlineV2.Entities;
using JCarrollOnlineV2.ViewModels;
using Microsoft.AspNet.Identity;
using NLog;
using Omu.ValueInjecter;
using System.Collections.Generic;
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
            _data = dataContext ?? new JCarrollOnlineV2Db();
        }

        // GET: Users
        public async Task<ActionResult> Index()
        {
            UsersIndexViewModel uiVM = new UsersIndexViewModel();

            uiVM.PageTitle = "Users";

            var users = await _data.ApplicationUser.Include("Following").Include("Followers").ToListAsync();

            foreach (var user in users)
            {
                UserItemViewModel uivm = new UserItemViewModel(logger);
                //uiVM.User = new ApplicationUserViewModel();

                uivm.User.InjectFrom(user);
                uivm.MicroPostsAuthored = await _data.ApplicationUser.Include("MicroPosts").Where(u => u.Id == user.Id).Select(u => u.MicroPosts).CountAsync();
                uiVM.Users.Add(uivm);
            }

            return View(uiVM);
        }

        // GET: Users/Details/5
        public async Task<ActionResult> Details(string userId)
        {
            UserDetailViewModel udVM = new UserDetailViewModel();

            string currentUserId = User.Identity.GetUserId();
            ApplicationUser currentUser = await _data.ApplicationUser.FirstOrDefaultAsync(x => x.Id == currentUserId);

            ApplicationUser user = await _data.ApplicationUser.Include("Following").Include("Followers").SingleAsync(m => m.Id == userId);
            udVM.UserInfoVM.User.InjectFrom(user);

            udVM.UserInfoVM.MicroPostEmailNotifications = user.MicroPostEmailNotifications;
            udVM.UserInfoVM.MicroPostSMSNotifications = user.MicroPostSMSNotifications;
            udVM.UserInfoVM.UserId = currentUserId;

            udVM.UserStatsVM = new UserStatsViewModel();
            udVM.UserStatsVM.UsersFollowing = new UserFollowingViewModel();
            udVM.UserStatsVM.User.InjectFrom(user);
            foreach (var item in user.Following)
            {
                UserItemViewModel uiVM = new UserItemViewModel(logger);
                uiVM.User.InjectFrom(item);
                uiVM.MicroPostsAuthored = await _data.MicroPost.Where(u => u.Author.Id == item.Id).CountAsync();
                udVM.UserStatsVM.UsersFollowing.Users.Add(uiVM);
            }
            udVM.UserStatsVM.UserFollowers = new UserFollowersViewModel();
            foreach (var item in user.Followers)
            {
                UserItemViewModel uiVM = new UserItemViewModel(logger);
                uiVM.User.InjectFrom(item);
                uiVM.MicroPostsAuthored = await _data.MicroPost.Where(u => u.Author.Id == item.Id).CountAsync();
                udVM.UserStatsVM.UserFollowers.Users.Add(uiVM);
            }
            return View(udVM);
        }

        public async Task<ActionResult> Following(string userId)
        {
            UserDetailViewModel udVM = new UserDetailViewModel();
            udVM.PageTitle = "Following";
            udVM.UserInfoVM = new UserItemViewModel(logger);

            ApplicationUser user = await _data.ApplicationUser.Include("Following").Include("Followers").SingleAsync(m => m.Id == userId);
            udVM.UserStatsVM = new UserStatsViewModel();
            udVM.User.InjectFrom(user);
            udVM.UserInfoVM.User.InjectFrom(user);
            udVM.UserStatsVM.User.InjectFrom(user);

            foreach (var item in user.Following)
            {
                UserItemViewModel uiVM = new UserItemViewModel(logger);
                uiVM.User.InjectFrom(item);
                uiVM.MicroPostsAuthored = item.MicroPosts.Count();
                udVM.UserStatsVM.UsersFollowing.Users.Add(uiVM);
            }
            foreach (var item in user.Followers)
            {
                UserItemViewModel uiVM = new UserItemViewModel(logger);
                uiVM.InjectFrom(item);
                uiVM.MicroPostsAuthored = item.MicroPosts.Count();
                udVM.UserStatsVM.UserFollowers.Users.Add(uiVM);
            }
            return View("Show_Follow", udVM);
        }

        public async Task<ActionResult> Followed(string userId)
        {
            UserDetailViewModel udVM = new UserDetailViewModel();
            udVM.PageTitle = "Followers";
            udVM.UserInfoVM = new UserItemViewModel(logger);

            ApplicationUser user = await _data.ApplicationUser.Include("Following").Include("Followers").SingleAsync(m => m.Id == userId);
            udVM.UserStatsVM = new UserStatsViewModel();
            udVM.User.InjectFrom(user);
            udVM.UserInfoVM.User.InjectFrom(user);
            udVM.UserStatsVM.User.InjectFrom(user);

            foreach (var item in user.Following)
            {
                UserItemViewModel uiVM = new UserItemViewModel(logger);
                uiVM.User.InjectFrom(item);
                uiVM.MicroPostsAuthored = item.MicroPosts.Count();
                udVM.UserStatsVM.UsersFollowing.Users.Add(uiVM);
            }
            foreach (var item in user.Followers)
            {
                UserItemViewModel uiVM = new UserItemViewModel(logger);
                uiVM.User.InjectFrom(item);
                uiVM.MicroPostsAuthored = item.MicroPosts.Count();
                udVM.UserStatsVM.UserFollowers.Users.Add(uiVM);
            }
            return View("Show_Follow", udVM);
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
        public async Task<ActionResult> UserSettings([Bind(Include = "UserId,MicroPostEmailNotifications,MicroPostSMSNotifications")] UserItemViewModel auVM)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = await _data.ApplicationUser.FirstOrDefaultAsync(x => x.Id == auVM.UserId);

                user.MicroPostEmailNotifications = auVM.MicroPostEmailNotifications;
                user.MicroPostSMSNotifications = auVM.MicroPostSMSNotifications;
                await _data.SaveChangesAsync();
            }
            return RedirectToAction("Details", new { userid = auVM.UserId });
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

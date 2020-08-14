using JCarrollOnlineV2.DataContexts;
using JCarrollOnlineV2.Entities;
using JCarrollOnlineV2.EntityFramework;
using JCarrollOnlineV2.ViewModels.Users;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security.Provider;
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
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private JCarrollOnlineV2DbContext Data { get; set; }

        public UsersController()
            : this(null)
        {

        }

        public UsersController(JCarrollOnlineV2DbContext dataContext)
        {
            Data = dataContext ?? new JCarrollOnlineV2DbContext();
        }

        // GET: Users
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            UsersIndexViewModel usersIndexViewModel = new UsersIndexViewModel
            {
                PageTitle = "Users"
            };

            System.Collections.Generic.List<ApplicationUser> users = await Data.ApplicationUser.Include("Following").Include("Followers").ToListAsync().ConfigureAwait(false);

            foreach (ApplicationUser user in users)
            {
                UserItemViewModel userItemViewModel = new UserItemViewModel(_logger);

                userItemViewModel.User.InjectFrom(user);
                userItemViewModel.MicroPostsAuthored = await Data.ApplicationUser.Include("MicroPosts").Where(u => u.Id == user.Id).Select(u => u.MicroPosts).CountAsync().ConfigureAwait(false);
                usersIndexViewModel.Users.Add(userItemViewModel);
            }

            return View(usersIndexViewModel);
        }

        // GET: Users/Details/5
        [HttpGet]
        public async Task<ActionResult> Details(string userId)
        {
            UserDetailViewModel userDetailViewModel = new UserDetailViewModel();

            string currentUserId = User.Identity.GetUserId();
            
            if(userId == null)
            {
                userId = currentUserId;
            }

            ApplicationUser currentUser = await Data.ApplicationUser.FirstOrDefaultAsync(x => x.Id == currentUserId).ConfigureAwait(false);
            ApplicationUser user = await Data.ApplicationUser.Include("Following").Include("Followers").FirstOrDefaultAsync(m => m.Id == userId).ConfigureAwait(false);

            if (user != null)
            {
                userDetailViewModel.UserInfoViewModel.User.InjectFrom(user);

                userDetailViewModel.UserInfoViewModel.MicroPostEmailNotifications = user.MicroPostEmailNotifications;
                //udVM.UserInfoVM.MicroPostSMSNotifications = user.MicroPostSMSNotifications;
                userDetailViewModel.UserInfoViewModel.UserId = currentUserId;

                userDetailViewModel.UserStatsViewModel = new UserStatsViewModel
                {
                    UsersFollowing = new UserFollowingViewModel()
                };

                userDetailViewModel.UserStatsViewModel.User.InjectFrom(user);

                foreach (ApplicationUser following in user.Following)
                {
                    UserItemViewModel userItemViewModel = new UserItemViewModel(_logger);

                    userItemViewModel.User.InjectFrom(following);
                    userItemViewModel.UserId = following.Id;
                    userItemViewModel.MicroPostsAuthored = await Data.MicroPost.Where(u => u.Author.Id == following.Id).CountAsync().ConfigureAwait(false);
                    userDetailViewModel.UserStatsViewModel.UsersFollowing.Users.Add(userItemViewModel);
                }

                userDetailViewModel.UserStatsViewModel.UserFollowers = new UserFollowersViewModel();

                foreach (ApplicationUser follower in user.Followers)
                {
                    UserItemViewModel userItemViewModel = new UserItemViewModel(_logger);

                    userItemViewModel.User.InjectFrom(follower);
                    userItemViewModel.UserId = follower.Id;
                    userItemViewModel.MicroPostsAuthored = await Data.MicroPost.Where(u => u.Author.Id == follower.Id).CountAsync().ConfigureAwait(false);
                    userDetailViewModel.UserStatsViewModel.UserFollowers.Users.Add(userItemViewModel);
                }
            }
            return View(userDetailViewModel);
        }

        [HttpGet]
        public async Task<ActionResult> Following(string userId)
        {
            UserDetailViewModel userDetailViewModel = new UserDetailViewModel
            {
                PageTitle = "Following",
                UserInfoViewModel = new UserItemViewModel(_logger)
            };

            ApplicationUser user = await Data.ApplicationUser.Include("Following").Include("Followers").SingleAsync(m => m.Id == userId).ConfigureAwait(false);

            userDetailViewModel.UserStatsViewModel = new UserStatsViewModel();
            userDetailViewModel.User.InjectFrom(user);
            userDetailViewModel.UserInfoViewModel.User.InjectFrom(user);
            userDetailViewModel.UserStatsViewModel.User.InjectFrom(user);

            foreach (ApplicationUser following in user.Following)
            {
                UserItemViewModel userItemViewModel = new UserItemViewModel(_logger);
                userItemViewModel.User.InjectFrom(following);
                userItemViewModel.MicroPostsAuthored = following.MicroPosts.Count;
                userDetailViewModel.UserStatsViewModel.UsersFollowing.Users.Add(userItemViewModel);
            }

            foreach (ApplicationUser follower in user.Followers)
            {
                UserItemViewModel userItemViewModel = new UserItemViewModel(_logger);
                userItemViewModel.InjectFrom(follower);
                userItemViewModel.MicroPostsAuthored = follower.MicroPosts.Count;
                userDetailViewModel.UserStatsViewModel.UserFollowers.Users.Add(userItemViewModel);
            }

            return View("Show_Follow", userDetailViewModel);
        }

        [HttpGet]
        public async Task<ActionResult> Followed(string userId)
        {
            UserDetailViewModel userDetailViewModel = new UserDetailViewModel
            {
                PageTitle = "Followers",
                UserInfoViewModel = new UserItemViewModel(_logger)
            };

            ApplicationUser user = await Data.ApplicationUser.Include("Following").Include("Followers").SingleAsync(m => m.Id == userId).ConfigureAwait(false);

            userDetailViewModel.UserStatsViewModel = new UserStatsViewModel();
            userDetailViewModel.User.InjectFrom(user);
            userDetailViewModel.UserInfoViewModel.User.InjectFrom(user);
            userDetailViewModel.UserStatsViewModel.User.InjectFrom(user);

            foreach (ApplicationUser following in user.Following)
            {
                UserItemViewModel userItemViewModel = new UserItemViewModel(_logger);

                userItemViewModel.User.InjectFrom(following);
                userItemViewModel.MicroPostsAuthored = following.MicroPosts.Count;
                userDetailViewModel.UserStatsViewModel.UsersFollowing.Users.Add(userItemViewModel);
            }

            foreach (ApplicationUser follower in user.Followers)
            {
                UserItemViewModel userItemViewModel = new UserItemViewModel(_logger);

                userItemViewModel.User.InjectFrom(follower);
                userItemViewModel.MicroPostsAuthored = follower.MicroPosts.Count;
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
                ApplicationUser currentUser = await Data.ApplicationUser.FirstOrDefaultAsync(x => x.Id == currentUserId).ConfigureAwait(false);
                ApplicationUser followingUser = await Data.ApplicationUser.FirstOrDefaultAsync(x => x.Id == followUser.UserId).ConfigureAwait(false);

                currentUser.Following.Add(followingUser);
                await Data.SaveChangesAsync().ConfigureAwait(false);
            }

            return RedirectToAction("Details", new { userid = followUser?.UserId });
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Unfollow")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Unfollow([Bind(Include = "UserId")]  UserItemViewModel followUser)
        {

            if (ModelState.IsValid)
            {
                string currentUserId = User.Identity.GetUserId();
                ApplicationUser currentUser = await Data.ApplicationUser.FirstOrDefaultAsync(x => x.Id == currentUserId).ConfigureAwait(false);
                ApplicationUser followingUser = await Data.ApplicationUser.FirstOrDefaultAsync(x => x.Id == followUser.UserId).ConfigureAwait(false);

                currentUser.Following.Remove(followingUser);
                await Data.SaveChangesAsync().ConfigureAwait(false);
            }

            return RedirectToAction("Details", new { userid = followUser?.UserId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UserSettings([Bind(Include = "UserId,MicroPostEmailNotifications,MicroPostSmsNotifications")] UserItemViewModel userItemViewModel)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = await Data.ApplicationUser.FirstOrDefaultAsync(x => x.Id == userItemViewModel.UserId).ConfigureAwait(false);

                if (userItemViewModel != null)
                {
                    user.MicroPostEmailNotifications = userItemViewModel.MicroPostEmailNotifications;
                    //user.MicroPostSMSNotifications = auVM.MicroPostSMSNotifications;
                    await Data.SaveChangesAsync().ConfigureAwait(false);
                }
            }

            return RedirectToAction("Details", new { userid = userItemViewModel?.UserId });
        }

        // GET: Users/Create
        [HttpGet]
        public async Task<ActionResult> Create()
        {
            return await Task.Run<ActionResult>(() =>
            {
                return View();
            }).ConfigureAwait(false);

        }

        //// POST: Users/Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Create()
        //{
        //    return await Task.Run<ActionResult>(() =>
        //    {
        //        return RedirectToAction("Index");
        //    }).ConfigureAwait(false);
        //}

        // GET: Users/Edit/5
        [HttpGet]
        public async Task<ActionResult> Edit()
        {
            return await Task.Run<ActionResult>(() =>
            {
                return View();
            }).ConfigureAwait(false);
        }

        // POST: Users/Edit/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Edit(int id, FormCollection collection)
        //{
        //    return await Task.Run<ActionResult>(() =>
        //    {
        //        return RedirectToAction("Index");
        //    }).ConfigureAwait(false);
        //}

        // GET: Users/Delete/5
        [HttpGet]
        public async Task<ActionResult> Delete()
        {
            return await Task.Run<ActionResult>(() =>
            {
                return View();
            }).ConfigureAwait(false);
        }

        // POST: Users/Delete/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Delete()
        //{
        //    return await Task.Run<ActionResult>(() =>
        //    {
        //        // TODO: Add delete logic here
        //        return RedirectToAction("Index");
        //    }).ConfigureAwait(false);
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
            }

            base.Dispose(disposing);
        }
    }
}

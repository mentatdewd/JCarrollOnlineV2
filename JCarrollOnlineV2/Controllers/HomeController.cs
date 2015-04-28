using JCarrollOnlineV2.DataContexts;
using JCarrollOnlineV2.Entities;
using JCarrollOnlineV2.ViewModels;
using Microsoft.AspNet.Identity;
using Omu.ValueInjecter;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using PagedList;
using System;

namespace JCarrollOnlineV2.Controllers
{
    public class HomeController : Controller
    {
        private IContext _data { get; set; }

        public HomeController()
            : this(null)
        {
        }

        public HomeController(IContext dataContext = null)
        {
            _data = dataContext ?? new JCarrollOnlineV2Db();
        }

        public async Task<ActionResult> Index(int? micropostPage)
        {
            HomeViewModel vm = new HomeViewModel();
            vm.Message = "JCarrollOnlineV2 Home - Index";
            vm.MicropostCreateVM = new MicropostCreateViewModel();
            vm.MicropostFeedVM = new MicropostFeedViewModel();
            vm.UserStatsVM = new UserStatsViewModel();
            vm.UserInfoVM = new UserItemViewModel();
            vm.BlogFeed = new BlogFeedViewModel();

            vm.UserStatsVM.UserFollowers = new UserFollowersViewModel();
            vm.UserStatsVM.UsersFollowing = new UserFollowingViewModel();

            var blogItems = await _data.BlogItems.OrderByDescending(m => m.UpdatedAt).ToListAsync();

            foreach (var item in blogItems)
            {
                BlogFeedItemViewModel bfi = new BlogFeedItemViewModel();
                bfi.InjectFrom(item);
                bfi.Author.InjectFrom(item.Author);
                vm.BlogFeed.BlogFeedItemVMs.Add(bfi);
            }

            Task<RssFeedViewModel> rss = ControllerHelpers.UpdateRssAsync();

            if (User != null && User.Identity.IsAuthenticated == true)
            {
                string currentUserId = User.Identity.GetUserId();
                ApplicationUser user = await _data.Users.Include("Following").Include("Followers").Include("Microposts").SingleAsync(u => u.Id == currentUserId);

                vm.UserInfoVM.User.InjectFrom(user);
                vm.UserInfoVM.MicropostsAuthored = user.Microposts.Count();
                vm.UserStatsVM.User.InjectFrom(user);

                foreach (ApplicationUser item in user.Following)
                {
                    UserItemViewModel uivm = new UserItemViewModel();
                    uivm.InjectFrom(item);
                    vm.UserStatsVM.UsersFollowing.Users.Add(uivm);

                    foreach (var micropost in item.Microposts)
                    {
                        MicropostFeedItemViewModel mpVM = new MicropostFeedItemViewModel();
                        mpVM.InjectFrom(micropost);
                        mpVM.Author.InjectFrom(micropost.Author);
                        mpVM.TimeAgo = mpVM.CreatedAt.ToUniversalTime().ToString("o");
                        vm.MicropostFeedVM.MicropostFeedItems.Add(mpVM);
                    }
                }
                foreach (var item in user.Followers)
                {
                    UserItemViewModel uivm = new UserItemViewModel();
                    uivm.InjectFrom(item);
                    uivm.MicropostsAuthored = await _data.Users.Include("Microposts").Where(u => u.Id == item.Id).Select(u => u.Microposts).CountAsync();
                    vm.UserStatsVM.UserFollowers.Users.Add(uivm);
                }
                foreach (var micropost in user.Microposts)
                {
                    MicropostFeedItemViewModel mpVM = new MicropostFeedItemViewModel();
                    mpVM.InjectFrom(micropost);
                    mpVM.Author.InjectFrom(micropost.Author);
                    mpVM.TimeAgo = mpVM.CreatedAt.ToUniversalTime().ToString("o");
                    vm.MicropostFeedVM.MicropostFeedItems.Add(mpVM);
                }
                var micropostPageNumber = micropostPage ?? 1;
                vm.MicropostFeedVM.OnePageOfMicroposts = vm.MicropostFeedVM.MicropostFeedItems.ToPagedList(micropostPageNumber, 5);

                vm.RssFeedVM = await rss;
            }
            vm.PageContainer = "Home";
            return View(vm);
        }

        public ActionResult About()
        {
            AboutViewModel vm = new AboutViewModel();

            vm.Message = "About JCarrollOnlineV2";
            vm.PageContainer = "AboutPage";
            return View(vm);
        }

        public ActionResult Contact()
        {
            ContactViewModel vm = new ContactViewModel();

            vm.Message = "JCarrollOnlineV2 Contact";
            vm.PageContainer = "ContactPater";
            return View(vm);
        }

        public async Task<ActionResult> Welcome()
        {
            HomeViewModel vm = new HomeViewModel();
            vm.Message = "JCarrollOnlineV2 Home - Welcome";
            vm.PageContainer = "Welcome";
            if (Request.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View("Welcome", "_LayoutWelcome", vm);
            }
        }
    }
}
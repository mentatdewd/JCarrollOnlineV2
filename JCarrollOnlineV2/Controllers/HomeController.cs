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
            HomeViewModel hVM = new HomeViewModel();
            hVM.Message = "JCarrollOnlineV2 Home - Index";
            hVM.MicropostCreateVM = new MicropostCreateViewModel();
            hVM.MicropostFeedVM = new MicropostFeedViewModel();
            hVM.UserStatsVM = new UserStatsViewModel();
            hVM.UserInfoVM = new UserItemViewModel();
            hVM.BlogFeed = new BlogFeedViewModel();

            hVM.UserStatsVM.UserFollowers = new UserFollowersViewModel();
            hVM.UserStatsVM.UsersFollowing = new UserFollowingViewModel();

            var blogItems = await _data.BlogItems.Include("BlogItemComments").OrderByDescending(m => m.UpdatedAt).ToListAsync();

            foreach (var item in blogItems)
            {
                BlogFeedItemViewModel bfi = new BlogFeedItemViewModel();
                bfi.InjectFrom(item);
                bfi.Comments.BlogItemId = item.Id;
                bfi.Author.InjectFrom(item.Author);
                foreach(var comment in item.BlogItemComments.ToList())
                {
                    BlogCommentItemViewModel bciVM = new BlogCommentItemViewModel();
                    bciVM.InjectFrom(comment);
                    bciVM.BlogItemId = comment.BlogItem.Id;
                    bciVM.TimeAgo = bciVM.CreatedAt.ToUniversalTime().ToString("o");
                    bfi.Comments.BlogComments.Add(bciVM);
                }
                hVM.BlogFeed.BlogFeedItemVMs.Add(bfi);
            }

            Task<RssFeedViewModel> rss = ControllerHelpers.UpdateRssAsync();

            if (User != null && User.Identity.IsAuthenticated == true)
            {
                string currentUserId = User.Identity.GetUserId();
                ApplicationUser user = await _data.Users.Include("Following").Include("Followers").Include("Microposts").SingleAsync(u => u.Id == currentUserId);

                hVM.UserInfoVM.User.InjectFrom(user);
                hVM.UserInfoVM.MicropostsAuthored = user.Microposts.Count();
                hVM.UserStatsVM.User.InjectFrom(user);

                foreach (ApplicationUser item in user.Following)
                {
                    UserItemViewModel uiVM = new UserItemViewModel();
                    uiVM.InjectFrom(item);
                    hVM.UserStatsVM.UsersFollowing.Users.Add(uiVM);

                    foreach (var micropost in item.Microposts)
                    {
                        MicropostFeedItemViewModel mpVM = new MicropostFeedItemViewModel();
                        mpVM.InjectFrom(micropost);
                        mpVM.Author.InjectFrom(micropost.Author);
                        mpVM.TimeAgo = mpVM.CreatedAt.ToUniversalTime().ToString("o");
                        hVM.MicropostFeedVM.MicropostFeedItems.Add(mpVM);
                    }
                }
                foreach (var item in user.Followers)
                {
                    UserItemViewModel uiVM = new UserItemViewModel();
                    uiVM.InjectFrom(item);
                    uiVM.MicropostsAuthored = await _data.Users.Include("Microposts").Where(u => u.Id == item.Id).Select(u => u.Microposts).CountAsync();
                    hVM.UserStatsVM.UserFollowers.Users.Add(uiVM);
                }
                foreach (var micropost in user.Microposts)
                {
                    MicropostFeedItemViewModel mpVM = new MicropostFeedItemViewModel();
                    mpVM.InjectFrom(micropost);
                    mpVM.Author.InjectFrom(micropost.Author);
                    mpVM.TimeAgo = mpVM.CreatedAt.ToUniversalTime().ToString("o");
                    hVM.MicropostFeedVM.MicropostFeedItems.Add(mpVM);
                }
                var micropostPageNumber = micropostPage ?? 1;
                hVM.MicropostFeedVM.OnePageOfMicroposts = hVM.MicropostFeedVM.MicropostFeedItems.OrderByDescending(m => m.CreatedAt).ToPagedList(micropostPageNumber, 4);

                hVM.RssFeedVM = await rss;
            }
            hVM.PageContainer = "Home";
            return View(hVM);
        }

        public ActionResult About()
        {
            AboutViewModel aVM = new AboutViewModel();

            aVM.Message = "About JCarrollOnlineV2";
            aVM.PageContainer = "AboutPage";
            return View(aVM);
        }

        public ActionResult Contact()
        {
            ContactViewModel cVM = new ContactViewModel();

            cVM.Message = "JCarrollOnlineV2 Contact";
            cVM.PageContainer = "ContactPater";
            return View(cVM);
        }

        public async Task<ActionResult> Welcome()
        {
            HomeViewModel hVM = new HomeViewModel();
            hVM.Message = "JCarrollOnlineV2 Home - Welcome";
            hVM.PageContainer = "Welcome";
            if (Request.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View("Welcome", "_LayoutWelcome", hVM);
            }
        }
    }
}
using JCarrollOnlineV2.DataContexts;
using JCarrollOnlineV2.Entities;
using JCarrollOnlineV2.ViewModels;
using Microsoft.AspNet.Identity;
using NLog;
using Omu.ValueInjecter;
using PagedList;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace JCarrollOnlineV2.Controllers
{
    public class HomeController : Controller
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private IJCarrollOnlineV2Context _data { get; set; }

        public HomeController()
            : this(null)
        {
        }

        public HomeController(IJCarrollOnlineV2Context dataContext = null)
        {
            _data = dataContext ?? new JCarrollOnlineV2Db();
        }

        public async Task<ActionResult> Index(int? micropostPage)
        {
            logger.Info("In Home/Index");
            HomeViewModel hVM = new HomeViewModel();
            hVM.Message = "JCarrollOnlineV2 Home - Index";
            hVM.MicroPostCreateVM = new MicroPostCreateViewModel();
            hVM.MicroPostFeedVM = new MicroPostFeedViewModel();
            hVM.UserStatsVM = new UserStatsViewModel();
            hVM.UserInfoVM = new UserItemViewModel();
            hVM.BlogFeed = new BlogFeedViewModel();

            hVM.UserStatsVM.UserFollowers = new UserFollowersViewModel();
            hVM.UserStatsVM.UsersFollowing = new UserFollowingViewModel();

            logger.Info("Checking for blog entries");
            var blogItems = await _data.BlogItem.Include("BlogItemComments").OrderByDescending(m => m.UpdatedAt).ToListAsync();

            logger.Info("Processing blog entries");

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

            logger.Info("Processing rss");

            Task<RssFeedViewModel> rss = ControllerHelpers.UpdateRssAsync();

            if (User != null && User.Identity.IsAuthenticated == true)
            {
                string currentUserId = User.Identity.GetUserId();
                ApplicationUser user = await _data.ApplicationUser.Include("Following").Include("Followers").Include("MicroPosts").SingleAsync(u => u.Id == currentUserId);

                hVM.UserInfoVM.User.InjectFrom(user);
                hVM.UserInfoVM.MicroPostsAuthored = user.MicroPosts.Count();
                hVM.UserStatsVM.User.InjectFrom(user);

                logger.Info("Processing followings");
                foreach (ApplicationUser item in user.Following)
                {
                    UserItemViewModel uiVM = new UserItemViewModel();
                    uiVM.InjectFrom(item);
                    hVM.UserStatsVM.UsersFollowing.Users.Add(uiVM);

                    foreach (var microPost in item.MicroPosts)
                    {
                        MicroPostFeedItemViewModel mpVM = new MicroPostFeedItemViewModel();
                        mpVM.InjectFrom(microPost);
                        mpVM.Author.InjectFrom(microPost.Author);
                        mpVM.TimeAgo = mpVM.CreatedAt.ToUniversalTime().ToString("o");
                        hVM.MicroPostFeedVM.MicroPostFeedItems.Add(mpVM);
                    }
                }
                logger.Info("Processing followers");
                foreach (var item in user.Followers)
                {
                    UserItemViewModel uiVM = new UserItemViewModel();
                    uiVM.InjectFrom(item);
                    uiVM.MicroPostsAuthored = await _data.ApplicationUser.Include("MicroPosts").Where(u => u.Id == item.Id).Select(u => u.MicroPosts).CountAsync();
                    hVM.UserStatsVM.UserFollowers.Users.Add(uiVM);
                }
                logger.Info("Processing microPosts");
                foreach (var micropost in user.MicroPosts)
                {
                    MicroPostFeedItemViewModel mpVM = new MicroPostFeedItemViewModel();
                    mpVM.InjectFrom(micropost);
                    mpVM.Author.InjectFrom(micropost.Author);
                    mpVM.TimeAgo = mpVM.CreatedAt.ToUniversalTime().ToString("o");
                    hVM.MicroPostFeedVM.MicroPostFeedItems.Add(mpVM);
                }
                var micropostPageNumber = micropostPage ?? 1;
                hVM.MicroPostFeedVM.OnePageOfMicroPosts = hVM.MicroPostFeedVM.MicroPostFeedItems.OrderByDescending(m => m.CreatedAt).ToPagedList(micropostPageNumber, 4);

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

            return await Task.Run<ActionResult>(() =>
            {
                if (Request.IsAuthenticated)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return View("Welcome", "_LayoutWelcome", hVM);
                }
            });
        }
    }
}
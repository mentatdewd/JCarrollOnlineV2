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
            _data = dataContext ?? new JCarrollOnlineV2Connection();
        }

        public async Task<ActionResult> Index(int? micropostPage)
        {
            logger.Info("In Home/Index");
            HomeViewModel homeViewModel = new HomeViewModel();
            homeViewModel.Message = "JCarrollOnlineV2 Home - Index";
            homeViewModel.MicroPostCreateVM = new MicroPostCreateViewModel();
            homeViewModel.MicroPostFeedVM = new MicroPostFeedViewModel();
            homeViewModel.UserStatsVM = new UserStatsViewModel();
            homeViewModel.UserInfoVM = new UserItemViewModel(logger);
            homeViewModel.BlogFeed = new BlogFeedViewModel();

            homeViewModel.UserStatsVM.UserFollowers = new UserFollowersViewModel();
            homeViewModel.UserStatsVM.UsersFollowing = new UserFollowingViewModel();

            logger.Info("Checking for blog entries");
            var blogItems = await _data.BlogItem.Include("BlogItemComments").OrderByDescending(m => m.UpdatedAt).ToListAsync();

            logger.Info("Processing blog entries");

            foreach (var item in blogItems)
            {
                BlogFeedItemViewModel blogFeedItemViewModel = new BlogFeedItemViewModel();

                blogFeedItemViewModel.InjectFrom(item);
                blogFeedItemViewModel.Comments.BlogItemId = item.Id;
                blogFeedItemViewModel.Author.InjectFrom(item.Author);

                foreach(var comment in item.BlogItemComments.ToList())
                {
                    BlogCommentItemViewModel blogCommentItemViewModel = new BlogCommentItemViewModel();

                    blogCommentItemViewModel.InjectFrom(comment);
                    blogCommentItemViewModel.BlogItemId = comment.BlogItem.Id;
                    blogCommentItemViewModel.TimeAgo = blogCommentItemViewModel.CreatedAt.ToUniversalTime().ToString("o");
                    blogFeedItemViewModel.Comments.BlogComments.Add(blogCommentItemViewModel);
                }

                homeViewModel.BlogFeed.BlogFeedItemVMs.Add(blogFeedItemViewModel);
            }

            logger.Info("Processing rss");

            Task<RssFeedViewModel> rss = ControllerHelpers.UpdateRssAsync();

            if (User != null && User.Identity.IsAuthenticated == true)
            {
                string currentUserId = User.Identity.GetUserId();
                ApplicationUser user = await _data.ApplicationUser.Include("Following").Include("Followers").Include("MicroPosts").SingleAsync(u => u.Id == currentUserId);

                homeViewModel.UserInfoVM.User.InjectFrom(user);
                homeViewModel.UserInfoVM.UserId = user.Id;
                homeViewModel.UserInfoVM.MicroPostsAuthored = user.MicroPosts.Count();
                homeViewModel.UserStatsVM.User.InjectFrom(user);

                logger.Info("Processing followings");

                foreach (ApplicationUser item in user.Following)
                {
                    UserItemViewModel userItemViewModel = new UserItemViewModel(logger);

                    userItemViewModel.InjectFrom(item);
                    homeViewModel.UserStatsVM.UsersFollowing.Users.Add(userItemViewModel);

                    foreach (var microPost in item.MicroPosts)
                    {
                        MicroPostFeedItemViewModel microPostFeedItemViewModel = new MicroPostFeedItemViewModel();
                        microPostFeedItemViewModel.InjectFrom(microPost);
                        microPostFeedItemViewModel.Author.InjectFrom(microPost.Author);
                        microPostFeedItemViewModel.TimeAgo = microPostFeedItemViewModel.CreatedAt.ToUniversalTime().ToString("o");
                        homeViewModel.MicroPostFeedVM.MicroPostFeedItems.Add(microPostFeedItemViewModel);
                    }
                }

                logger.Info("Processing followers");

                foreach (var item in user.Followers)
                {
                    UserItemViewModel userItemViewModel = new UserItemViewModel(logger);

                    userItemViewModel.InjectFrom(item);
                    userItemViewModel.MicroPostsAuthored = await _data.ApplicationUser.Include("MicroPosts").Where(u => u.Id == item.Id).Select(u => u.MicroPosts).CountAsync();
                    homeViewModel.UserStatsVM.UserFollowers.Users.Add(userItemViewModel);
                }

                logger.Info("Processing microPosts");

                foreach (var micropost in user.MicroPosts)
                {
                    MicroPostFeedItemViewModel microPostFeedItemViewModel = new MicroPostFeedItemViewModel();

                    microPostFeedItemViewModel.InjectFrom(micropost);
                    microPostFeedItemViewModel.Author.InjectFrom(micropost.Author);
                    microPostFeedItemViewModel.TimeAgo = microPostFeedItemViewModel.CreatedAt.ToUniversalTime().ToString("o");
                    homeViewModel.MicroPostFeedVM.MicroPostFeedItems.Add(microPostFeedItemViewModel);
                }

                var micropostPageNumber = micropostPage ?? 1;

                homeViewModel.MicroPostFeedVM.OnePageOfMicroPosts = homeViewModel.MicroPostFeedVM.MicroPostFeedItems.OrderByDescending(m => m.CreatedAt).ToPagedList(micropostPageNumber, 4);

                logger.Info("awaiting rss");
                homeViewModel.RssFeedVM = await rss;
            }

            homeViewModel.PageContainer = "Home";
            logger.Info("Navigating to homepage");

            return View(homeViewModel);
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
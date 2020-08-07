using JCarrollOnlineV2.DataContexts;
using JCarrollOnlineV2.Entities;
using JCarrollOnlineV2.EntityFramework;
using JCarrollOnlineV2.ViewModels;
using JCarrollOnlineV2.ViewModels.Blog;
using JCarrollOnlineV2.ViewModels.MicroPosts;
using JCarrollOnlineV2.ViewModels.Rss;
using JCarrollOnlineV2.ViewModels.Users;
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

        private JCarrollOnlineV2DbContext _data { get; set; }

        public HomeController()
        {
            _data = new JCarrollOnlineV2DbContext();
        }

        public async Task<ActionResult> Index(int? microPostPage)
        {
            logger.Info("In Home/Index");
            HomeViewModel homeViewModel = new HomeViewModel
            {
                Message = "JCarrollOnlineV2 Home - Index",
                MicroPostCreateViewModel = new MicroPostCreateViewModel(),
                MicroPostFeedViewModel = new MicroPostFeedViewModel(),
                UserStatsViewModel = new UserStatsViewModel(),
                UserInfoViewModel = new UserItemViewModel(logger),
                BlogFeed = new BlogFeedViewModel()
            };

            homeViewModel.UserStatsViewModel.UserFollowers = new UserFollowersViewModel();
            homeViewModel.UserStatsViewModel.UsersFollowing = new UserFollowingViewModel();

            logger.Info("Checking for blog entries");
            System.Collections.Generic.List<BlogItem> blogItems = await _data.BlogItem.Include("BlogItemComments").OrderByDescending(m => m.UpdatedAt).ToListAsync();

            homeViewModel.LatestForumThreadsViewModel = new LatestForumThreadsViewModel();
            System.Collections.Generic.List<ThreadEntry> threads = await _data.ForumThreadEntry.Include(forumThreadEntry => forumThreadEntry.Forum).OrderByDescending(threadEntry => threadEntry.UpdatedAt).Take(5).ToListAsync();

            foreach(ThreadEntry thread in threads)
            {
                LatestForumThreadItemViewModel latestForumThreadItemViewModel = new LatestForumThreadItemViewModel();

                latestForumThreadItemViewModel.ThreadTitle = thread.Title;
                latestForumThreadItemViewModel.ForumTitle = thread.Forum.Title;
                latestForumThreadItemViewModel.ForumId = thread.Forum.Id;
                latestForumThreadItemViewModel.ThreadId = thread.Id;

                homeViewModel.LatestForumThreadsViewModel.LatestForumThreads.Add(latestForumThreadItemViewModel);
            }

            logger.Info("Processing blog entries");

            foreach (BlogItem item in blogItems)
            {
                BlogFeedItemViewModel blogFeedItemViewModel = new BlogFeedItemViewModel();

                blogFeedItemViewModel.InjectFrom(item);
                blogFeedItemViewModel.Comments.BlogItemId = item.Id;
                blogFeedItemViewModel.Author.InjectFrom(item.Author);

                foreach(BlogItemComment comment in item.BlogItemComments.ToList())
                {
                    BlogCommentItemViewModel blogCommentItemViewModel = new BlogCommentItemViewModel(item.Id);

                    blogCommentItemViewModel.InjectFrom(comment);
                    blogCommentItemViewModel.BlogItemId = comment.BlogItem.Id;
                    blogCommentItemViewModel.TimeAgo = blogCommentItemViewModel.CreatedAt.ToUniversalTime().ToString("o");
                    blogFeedItemViewModel.Comments.BlogComments.Add(blogCommentItemViewModel);
                }

                homeViewModel.BlogFeed.BlogFeedItemViewModels.Add(blogFeedItemViewModel);
            }

            logger.Info("Processing rss");

            Task<RssFeedViewModel> rss = ControllerHelpers.UpdateRssAsync();

            if (User != null && User.Identity.IsAuthenticated == true)
            {
                string currentUserId = User.Identity.GetUserId();
                ApplicationUser user = await _data.ApplicationUser.Include("Following").Include("Followers").Include("MicroPosts").SingleAsync(u => u.Id == currentUserId);

                homeViewModel.UserInfoViewModel.User.InjectFrom(user);
                homeViewModel.UserInfoViewModel.UserId = user.Id;
                homeViewModel.UserInfoViewModel.MicroPostsAuthored = user.MicroPosts.Count();
                homeViewModel.UserStatsViewModel.User.InjectFrom(user);

                logger.Info("Processing followings");

                foreach (ApplicationUser item in user.Following)
                {
                    UserItemViewModel userItemViewModel = new UserItemViewModel(logger);

                    userItemViewModel.InjectFrom(item);
                    homeViewModel.UserStatsViewModel.UsersFollowing.Users.Add(userItemViewModel);

                    foreach (MicroPost microPost in item.MicroPosts)
                    {
                        MicroPostFeedItemViewModel microPostFeedItemViewModel = new MicroPostFeedItemViewModel();

                        microPostFeedItemViewModel.InjectFrom(microPost);
                        microPostFeedItemViewModel.Author.InjectFrom(microPost.Author);
                        microPostFeedItemViewModel.TimeAgo = microPostFeedItemViewModel.CreatedAt.ToUniversalTime().ToString("o");
                        homeViewModel.MicroPostFeedViewModel.MicroPostFeedItems.Add(microPostFeedItemViewModel);
                    }
                }

                logger.Info("Processing followers");

                foreach (ApplicationUser item in user.Followers)
                {
                    UserItemViewModel userItemViewModel = new UserItemViewModel(logger);

                    userItemViewModel.InjectFrom(item);
                    userItemViewModel.MicroPostsAuthored = await _data.ApplicationUser.Include("MicroPosts").Where(u => u.Id == item.Id).Select(u => u.MicroPosts).CountAsync();
                    homeViewModel.UserStatsViewModel.UserFollowers.Users.Add(userItemViewModel);
                }

                logger.Info("Processing microPosts");

                foreach (MicroPost micropost in user.MicroPosts)
                {
                    MicroPostFeedItemViewModel microPostFeedItemViewModel = new MicroPostFeedItemViewModel();

                    microPostFeedItemViewModel.InjectFrom(micropost);
                    microPostFeedItemViewModel.Author.InjectFrom(micropost.Author);
                    microPostFeedItemViewModel.TimeAgo = microPostFeedItemViewModel.CreatedAt.ToUniversalTime().ToString("o");
                    homeViewModel.MicroPostFeedViewModel.MicroPostFeedItems.Add(microPostFeedItemViewModel);
                }

                int micropostPageNumber = microPostPage ?? 1;

                homeViewModel.MicroPostFeedViewModel.OnePageOfMicroPosts = homeViewModel.MicroPostFeedViewModel.MicroPostFeedItems.OrderByDescending(m => m.CreatedAt).ToPagedList(micropostPageNumber, 4);

                logger.Info("awaiting rss");
                homeViewModel.RssFeedViewModel = await rss;
            }

            homeViewModel.PageContainer = "Home";
            logger.Info("Navigating to homepage");

            return View(homeViewModel);
        }

        public ActionResult About()
        {
            AboutViewModel aboutViewModel = new AboutViewModel
            {
                Message = "About JCarrollOnlineV2",
                PageContainer = "AboutPage"
            };

            return View(aboutViewModel);
        }

        public ActionResult Contact()
        {
            ContactViewModel contactViewModel = new ContactViewModel
            {
                Message = "JCarrollOnlineV2 Contact",
                PageContainer = "ContactPater"
            };

            return View(contactViewModel);
        }

        public async Task<ActionResult> Welcome()
        {
            HomeViewModel homeViewModel = new HomeViewModel
            {
                Message = "JCarrollOnlineV2 Home - Welcome",
                PageContainer = "Welcome"
            };

            return await Task.Run<ActionResult>(() =>
            {
                if (Request.IsAuthenticated)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return View("Welcome", "_LayoutWelcome", homeViewModel);
                }
            });
        }
    }
}
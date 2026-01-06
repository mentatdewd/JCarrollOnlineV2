using JCarrollOnlineV2.DataContexts;
using JCarrollOnlineV2.Entities;
using JCarrollOnlineV2.EntityFramework;
using JCarrollOnlineV2.ViewModels;
using JCarrollOnlineV2.ViewModels.Blog;
using JCarrollOnlineV2.ViewModels.Chat;
using JCarrollOnlineV2.ViewModels.MicroPosts;
using JCarrollOnlineV2.ViewModels.Rss;
using JCarrollOnlineV2.ViewModels.Users;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using NLog;
using Omu.ValueInjecter;
using PagedList;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace JCarrollOnlineV2.Controllers
{
    public class HomeController : Controller
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private JCarrollOnlineV2DbContext Data { get; set; }

        public HomeController()
        {
            Data = new JCarrollOnlineV2DbContext();
        }

        private IAuthenticationManager AuthenticationManager => HttpContext.GetOwinContext().Authentication;

        [HttpGet]
        public async Task<ActionResult> Index(int? microPostPage)
        {
            _logger.Info("In Home/Index");
            HomeViewModel homeViewModel = new HomeViewModel
            {
                Message = "JCarrollOnlineV2 Home - Index",
                MicroPostCreateViewModel = new MicroPostCreateViewModel(),
                MicroPostFeedViewModel = new MicroPostFeedViewModel(),
                UserStatsViewModel = new UserStatsViewModel(),
                UserInfoViewModel = new UserItemViewModel(_logger),
                BlogFeed = new BlogFeedViewModel()
            };

            homeViewModel.UserStatsViewModel.UserFollowers = new UserFollowersViewModel();
            homeViewModel.UserStatsViewModel.UsersFollowing = new UserFollowingViewModel();

            _logger.Info("Checking for blog entries");
            List<BlogItem> blogItems = await Data.BlogItem.Include("BlogItemComments").OrderByDescending(m => m.UpdatedAt).ToListAsync().ConfigureAwait(false);

            homeViewModel.LatestForumThreadsViewModel = new LatestForumThreadsViewModel();
            List<ThreadEntry> threads = await Data.ForumThreadEntry.Include(forumThreadEntry => forumThreadEntry.Forum).OrderByDescending(threadEntry => threadEntry.UpdatedAt).Take(5).ToListAsync().ConfigureAwait(false);

            foreach(ThreadEntry thread in threads)
            {
                LatestForumThreadItemViewModel latestForumThreadItemViewModel = new LatestForumThreadItemViewModel
                {
                    ThreadTitle = thread.Title,
                    ForumTitle = thread.Forum.Title,
                    ForumId = thread.Forum.Id,
                    ThreadId = thread.Id
                };

                homeViewModel.LatestForumThreadsViewModel.LatestForumThreads.Add(latestForumThreadItemViewModel);
            }

            _logger.Info("Processing blog entries");

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

            _logger.Info("Processing rss");

            Task<RssFeedViewModel> rss = ControllerHelpers.UpdateRssAsync();

            if (User != null && User.Identity.IsAuthenticated == true)
            {
                string currentUserId = User.Identity.GetUserId();
                
                // Use SingleOrDefaultAsync instead of SingleAsync to avoid exception
                ApplicationUser user = await Data.ApplicationUser
                    .Include("Following")
                    .Include("Followers")
                    .Include("MicroPosts")
                    .SingleOrDefaultAsync(u => u.Id == currentUserId)
                    .ConfigureAwait(false);

                // Check if user exists in database
                if (user == null)
                {
                    // User is authenticated but doesn't exist in database
                    // Log them out and redirect to registration
                    AuthenticationManager.SignOut();
                    return RedirectToAction("Register", "Account");
                }

                homeViewModel.UserInfoViewModel.User.InjectFrom(user);
                homeViewModel.UserInfoViewModel.UserId = user.Id;
                homeViewModel.UserInfoViewModel.MicroPostsAuthored = user.MicroPosts.Count;
                homeViewModel.UserStatsViewModel.User.InjectFrom(user);

                _logger.Info("Processing followings");

                foreach (ApplicationUser item in user.Following)
                {
                    UserItemViewModel userItemViewModel = new UserItemViewModel(_logger);

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

                _logger.Info("Processing followers");

                foreach (ApplicationUser item in user.Followers)
                {
                    UserItemViewModel userItemViewModel = new UserItemViewModel(_logger);

                    userItemViewModel.InjectFrom(item);
                    userItemViewModel.MicroPostsAuthored = await Data.ApplicationUser.Include("MicroPosts").Where(u => u.Id == item.Id).Select(u => u.MicroPosts).CountAsync().ConfigureAwait(false);
                    homeViewModel.UserStatsViewModel.UserFollowers.Users.Add(userItemViewModel);
                }

                _logger.Info("Processing microPosts");

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

                _logger.Info("awaiting rss");
                homeViewModel.RssFeedViewModel = await rss.ConfigureAwait(false);
            }

            homeViewModel.ChatViewModel = new ChatViewModel();

            // Load recent chat messages (last 50)
            List<ChatMessage> recentMessages = await Data.ChatMessages
                .Include(c => c.Author)
                .OrderByDescending(c => c.CreatedAt)
                .Take(50)
                .ToListAsync()
                .ConfigureAwait(false);

            foreach (ChatMessage msg in recentMessages.OrderBy(m => m.CreatedAt))
            {
                homeViewModel.ChatViewModel.RecentMessages.Add(new ChatMessageViewModel
                {
                    UserName = msg.Author.UserName,
                    Message = msg.Message,
                    TimeAgo = msg.CreatedAt.ToUniversalTime().ToString("o")
                });
            }

            homeViewModel.PageContainer = "Home";
            _logger.Info("Navigating to homepage");

            return View(homeViewModel);
        }

        [HttpGet]
        public ActionResult About()
        {
            AboutViewModel aboutViewModel = new AboutViewModel
            {
                Message = "About JCarrollOnlineV2",
                PageContainer = "AboutPage"
            };

            return View(aboutViewModel);
        }

        [HttpGet]
        public ActionResult Contact()
        {
            ContactViewModel contactViewModel = new ContactViewModel
            {
                Message = "JCarrollOnlineV2 Contact",
                PageContainer = "ContactPater"
            };

            return View(contactViewModel);
        }

        [HttpGet]
        public async Task<ActionResult> Welcome()
        {
            HomeViewModel homeViewModel = new HomeViewModel
            {
                Message = "JCarrollOnlineV2 Home - Welcome",
                PageContainer = "Welcome"
            };

            return await Task.Run<ActionResult>(() =>
            {
                return Request.IsAuthenticated ? RedirectToAction("Index", "Home") : (ActionResult)View("Welcome", "_LayoutWelcome", homeViewModel);
            }).ConfigureAwait(false);
        }
    }
}
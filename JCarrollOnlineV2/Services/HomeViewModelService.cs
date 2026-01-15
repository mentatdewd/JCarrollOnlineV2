using JCarrollOnlineV2.Entities;
using JCarrollOnlineV2.EntityFramework;
using JCarrollOnlineV2.ViewModels.Blog;
using JCarrollOnlineV2.ViewModels.Chat;
using JCarrollOnlineV2.ViewModels.Home;
using JCarrollOnlineV2.ViewModels.MicroPosts;
using JCarrollOnlineV2.ViewModels.Users;
using NLog;
using Omu.ValueInjecter;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace JCarrollOnlineV2.Services
{
    public class HomeViewModelService : IHomeViewModelService
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly JCarrollOnlineV2DbContext _context;
        private readonly IBlogViewModelService _blogViewModelService;
        private readonly IMicroPostViewModelService _microPostViewModelService;
        private readonly IUserStatsViewModelService _userStatsViewModelService;
        private readonly IRssService _rssService;

        public HomeViewModelService(
            JCarrollOnlineV2DbContext context,
            IBlogViewModelService blogViewModelService,
            IMicroPostViewModelService microPostViewModelService,
            IUserStatsViewModelService userStatsViewModelService,
            IRssService rssService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _blogViewModelService = blogViewModelService ?? throw new ArgumentNullException(nameof(blogViewModelService));
            _microPostViewModelService = microPostViewModelService ?? throw new ArgumentNullException(nameof(microPostViewModelService));
            _userStatsViewModelService = userStatsViewModelService ?? throw new ArgumentNullException(nameof(userStatsViewModelService));
            _rssService = rssService ?? throw new ArgumentNullException(nameof(rssService));
        }

        public async Task<HomeViewModel> BuildAnonymousHomeViewModelAsync()
        {
            _logger.Info("Building anonymous home view model");

            var homeViewModel = new HomeViewModel
            {
                Message = "JCarrollOnlineV2 Home - Index",
                BlogFeed = await _blogViewModelService.BuildBlogFeedViewModelAsync().ConfigureAwait(false),
                LatestForumThreadsViewModel = await BuildLatestForumThreadsViewModelAsync().ConfigureAwait(false),
                ChatViewModel = await BuildChatViewModelAsync().ConfigureAwait(false),
                PageContainer = "Home"
            };

            return homeViewModel;
        }

        public async Task<HomeViewModel> BuildAuthenticatedHomeViewModelAsync(string userId, int? microPostPage)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException("User ID cannot be null or empty", nameof(userId));
            }

            _logger.Info($"Building authenticated home view model for user {userId}");

            // Initialize base view model
            var homeViewModel = new HomeViewModel
            {
                Message = "JCarrollOnlineV2 Home - Index",
                MicroPostCreateViewModel = new MicroPostCreateViewModel(),
                MicroPostFeedViewModel = new MicroPostFeedViewModel(),
                UserStatsViewModel = new UserStatsViewModel(),
                UserInfoViewModel = new UserItemViewModel(_logger),
                BlogFeed = new BlogFeedViewModel(),
                PageContainer = "Home"
            };

            // Load user information
            var user = await _context.ApplicationUser
                .Include(u => u.MicroPosts)
                .AsNoTracking()
                .SingleOrDefaultAsync(u => u.Id == userId)
                .ConfigureAwait(false);

            if (user == null)
            {
                _logger.Warn($"User {userId} not found");
                return await BuildAnonymousHomeViewModelAsync().ConfigureAwait(false);
            }

            // Set user info
            homeViewModel.UserInfoViewModel.User.InjectFrom(user);
            homeViewModel.UserInfoViewModel.UserId = user.Id;
            homeViewModel.UserInfoViewModel.MicroPostsAuthored = user.MicroPosts.Count;

            // Build view models in parallel for better performance
            var blogFeedTask = _blogViewModelService.BuildBlogFeedViewModelAsync();
            var forumThreadsTask = BuildLatestForumThreadsViewModelAsync();
            var microPostFeedTask = _microPostViewModelService.BuildMicroPostFeedViewModelAsync(userId, microPostPage ?? 1, 4);
            var userStatsTask = _userStatsViewModelService.BuildUserStatsViewModelAsync(userId);
            var chatTask = BuildChatViewModelAsync();
            var rssTask = _rssService.GetRssFeedAsync();

            await Task.WhenAll(blogFeedTask, forumThreadsTask, microPostFeedTask, userStatsTask, chatTask, rssTask).ConfigureAwait(false);

            homeViewModel.BlogFeed = await blogFeedTask;
            homeViewModel.LatestForumThreadsViewModel = await forumThreadsTask;
            homeViewModel.MicroPostFeedViewModel = await microPostFeedTask;
            homeViewModel.UserStatsViewModel = await userStatsTask;
            homeViewModel.ChatViewModel = await chatTask;
            homeViewModel.RssFeedViewModel = await rssTask;

            _logger.Info("Successfully built authenticated home view model");

            return homeViewModel;
        }

        private async Task<LatestForumThreadsViewModel> BuildLatestForumThreadsViewModelAsync()
        {
            var latestForumThreadsViewModel = new LatestForumThreadsViewModel();

            try
            {
                List<ThreadEntry> threads = await _context.ForumThreadEntry
                    .Include(t => t.Forum)
                    .AsNoTracking()
                    .OrderByDescending(t => t.UpdatedAt)
                    .Take(5)
                    .ToListAsync()
                    .ConfigureAwait(false);

                foreach (ThreadEntry thread in threads)
                {
                    var latestForumThreadItemViewModel = new LatestForumThreadItemViewModel
                    {
                        ThreadTitle = thread.Title,
                        ForumTitle = thread.Forum.Title,
                        ForumId = thread.Forum.Id,
                        ThreadId = thread.Id
                    };

                    latestForumThreadsViewModel.LatestForumThreads.Add(latestForumThreadItemViewModel);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error building latest forum threads view model");
            }

            return latestForumThreadsViewModel;
        }

        private async Task<ChatViewModel> BuildChatViewModelAsync()
        {
            var chatViewModel = new ChatViewModel();

            try
            {
                List<ChatMessage> recentMessages = await _context.ChatMessages
                    .Include(c => c.Author)
                    .AsNoTracking()
                    .OrderByDescending(c => c.CreatedAt)
                    .Take(50)
                    .ToListAsync()
                    .ConfigureAwait(false);

                foreach (ChatMessage msg in recentMessages.OrderBy(m => m.CreatedAt))
                {
                    chatViewModel.RecentMessages.Add(new ChatMessageViewModel
                    {
                        UserName = msg.Author.UserName,
                        Message = msg.Message,
                        TimeAgo = msg.CreatedAt.ToUniversalTime().ToString("o")
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error building chat view model");
            }

            return chatViewModel;
        }
    }
}
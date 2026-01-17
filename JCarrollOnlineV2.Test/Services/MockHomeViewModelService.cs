using JCarrollOnlineV2.Services;
using JCarrollOnlineV2.ViewModels.Blog;
using JCarrollOnlineV2.ViewModels.Chat;
using JCarrollOnlineV2.ViewModels.ForumThreadEntries;
using JCarrollOnlineV2.ViewModels.Home;
using JCarrollOnlineV2.ViewModels.MicroPosts;
using JCarrollOnlineV2.ViewModels.Users;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JCarrollOnlineV2.Test.Services
{
    public class MockHomeViewModelService : IHomeViewModelService
    {
        public HomeViewModel MockAnonymousViewModel { get; set; }
        public HomeViewModel MockAuthenticatedViewModel { get; set; }

        public MockHomeViewModelService()
        {
            // Initialize with default empty view models
            MockAnonymousViewModel = CreateDefaultHomeViewModel();
            MockAuthenticatedViewModel = CreateDefaultHomeViewModel();
        }

        public Task<HomeViewModel> BuildAnonymousHomeViewModelAsync()
        {
            return Task.FromResult(MockAnonymousViewModel);
        }

        public Task<HomeViewModel> BuildAuthenticatedHomeViewModelAsync(string userId, int? microPostPage)
        {
            return Task.FromResult(MockAuthenticatedViewModel);
        }

        private HomeViewModel CreateDefaultHomeViewModel()
        {
            return new HomeViewModel
            {
                Message = "JCarrollOnlineV2 Home - Index",
                MicroPostCreateViewModel = new MicroPostCreateViewModel(),
                MicroPostFeedViewModel = new MicroPostFeedViewModel(),
                UserStatsViewModel = new UserStatsViewModel
                {
                    UserFollowers = new UserFollowersViewModel(),
                    UsersFollowing = new UserFollowingViewModel()
                },
                UserInfoViewModel = new UserItemViewModel(),
                BlogFeed = new BlogFeedViewModel(),
                LatestForumThreadsViewModel = new LatestForumThreadsViewModel(),
                ChatViewModel = new ChatViewModel(),
                PageContainer = "Home"
            };
        }
    }
}
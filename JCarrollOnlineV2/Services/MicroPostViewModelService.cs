using JCarrollOnlineV2.Entities;
using JCarrollOnlineV2.EntityFramework;
using JCarrollOnlineV2.ViewModels.MicroPosts;
using NLog;
using Omu.ValueInjecter;
using PagedList;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace JCarrollOnlineV2.Services
{
    public class MicroPostViewModelService : IMicroPostViewModelService
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly JCarrollOnlineV2DbContext _context;

        public MicroPostViewModelService(JCarrollOnlineV2DbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<MicroPostFeedViewModel> BuildMicroPostFeedViewModelAsync(string userId, int pageNumber, int pageSize)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException("User ID cannot be null or empty", nameof(userId));
            }

            _logger.Info($"Building micro post feed for user {userId}");

            var microPostFeedViewModel = new MicroPostFeedViewModel();

            try
            {
                // Load user with all relationships
                var user = await _context.ApplicationUser
                    .Include(u => u.Following.Select(f => f.MicroPosts.Select(mp => mp.Author)))
                    .Include(u => u.MicroPosts.Select(mp => mp.Author))
                    .AsNoTracking()
                    .SingleOrDefaultAsync(u => u.Id == userId)
                    .ConfigureAwait(false);

                if (user == null)
                {
                    _logger.Warn($"User {userId} not found");
                    return microPostFeedViewModel;
                }

                // Add user's own micro posts
                foreach (MicroPost micropost in user.MicroPosts)
                {
                    var microPostFeedItemViewModel = new MicroPostFeedItemViewModel();
                    microPostFeedItemViewModel.InjectFrom(micropost);
                    microPostFeedItemViewModel.Author.InjectFrom(micropost.Author);
                    microPostFeedItemViewModel.TimeAgo = microPostFeedItemViewModel.CreatedAt.ToUniversalTime().ToString("o");
                    microPostFeedViewModel.MicroPostFeedItems.Add(microPostFeedItemViewModel);
                }

                // Add micro posts from followed users
                foreach (ApplicationUser followedUser in user.Following)
                {
                    foreach (MicroPost microPost in followedUser.MicroPosts)
                    {
                        var microPostFeedItemViewModel = new MicroPostFeedItemViewModel();
                        microPostFeedItemViewModel.InjectFrom(microPost);
                        microPostFeedItemViewModel.Author.InjectFrom(microPost.Author);
                        microPostFeedItemViewModel.TimeAgo = microPostFeedItemViewModel.CreatedAt.ToUniversalTime().ToString("o");
                        microPostFeedViewModel.MicroPostFeedItems.Add(microPostFeedItemViewModel);
                    }
                }

                // Apply pagination
                microPostFeedViewModel.OnePageOfMicroPosts = microPostFeedViewModel.MicroPostFeedItems
                    .OrderByDescending(m => m.CreatedAt)
                    .ToPagedList(pageNumber, pageSize);

                _logger.Info($"Successfully built micro post feed with {microPostFeedViewModel.MicroPostFeedItems.Count} items");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error building micro post feed for user {userId}");
            }

            return microPostFeedViewModel;
        }
    }
}
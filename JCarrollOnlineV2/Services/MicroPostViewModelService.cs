using JCarrollOnlineV2.Entities;
using JCarrollOnlineV2.EntityFramework;
using JCarrollOnlineV2.ViewModels.MicroPosts;
using NLog;
using Omu.ValueInjecter;
using PagedList;
using System;
using System.Collections.Generic;
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
            _logger.Info($"MicroPostViewModelService constructed. Context is null: {_context == null}");
        }

        public async Task<MicroPostFeedViewModel> BuildMicroPostFeedViewModelAsync(string userId, int pageNumber, int pageSize)
        {
            _logger.Info($"===== BuildMicroPostFeedViewModelAsync CALLED ===== User: {userId}, Page: {pageNumber}, Size: {pageSize}");
            
            if (string.IsNullOrEmpty(userId))
            {
                _logger.Error("userId is null or empty!");
                throw new ArgumentException("User ID cannot be null or empty", nameof(userId));
            }

            MicroPostFeedViewModel microPostFeedViewModel = new MicroPostFeedViewModel();

            try
            {
                _logger.Info($"Context.MicroPost DbSet is null: {_context.MicroPost == null}");
                
                // Get the IDs of users that the current user is following
                // ApplicationUser_Id = current user, ApplicationUser_Id1 = user being followed
                List<string> followedUserIds = await _context.Database
                    .SqlQuery<string>(
                        "SELECT ApplicationUser_Id1 FROM ApplicationUserApplicationUser WHERE ApplicationUser_Id = @p0",
                        userId)
                    .ToListAsync()
                    .ConfigureAwait(false);

                _logger.Info($"User {userId} is following {followedUserIds.Count} users: [{string.Join(", ", followedUserIds)}]");

                // Create a list that includes the current user + all followed users
                List<string> userIdsToInclude = new List<string>(followedUserIds) { userId };

                _logger.Info($"Will load microposts from {userIdsToInclude.Count} users: [{string.Join(", ", userIdsToInclude)}]");

                // Get all microposts from the current user AND all followed users in one query
                List<MicroPost> allMicroPosts = await _context.MicroPost
                    .Include(mp => mp.Author)
                    .Where(mp => userIdsToInclude.Contains(mp.Author.Id))
                    .AsNoTracking()
                    .ToListAsync()
                    .ConfigureAwait(false);

                _logger.Info($"Database returned {allMicroPosts.Count} total microposts");

                if (allMicroPosts.Count == 0)
                {
                    _logger.Warn($"NO MICROPOSTS FOUND for user IDs: [{string.Join(", ", userIdsToInclude)}]");
                }

                // Convert to view models
                foreach (MicroPost microPost in allMicroPosts)
                {
                    MicroPostFeedItemViewModel microPostFeedItemViewModel = new MicroPostFeedItemViewModel();
                    microPostFeedItemViewModel.InjectFrom(microPost);
                    microPostFeedItemViewModel.Author.InjectFrom(microPost.Author);
                    microPostFeedItemViewModel.TimeAgo = microPostFeedItemViewModel.CreatedAt.ToUniversalTime().ToString("o");
                    microPostFeedViewModel.MicroPostFeedItems.Add(microPostFeedItemViewModel);
                    
                    _logger.Debug($"Added micropost ID {microPost.Id} from {microPost.Author?.UserName} created {microPost.CreatedAt}");
                }

                _logger.Info($"Converted {microPostFeedViewModel.MicroPostFeedItems.Count} microposts to view models");

                // Order by date descending BEFORE pagination
                var orderedMicroPosts = microPostFeedViewModel.MicroPostFeedItems
                    .OrderByDescending(m => m.CreatedAt)
                    .ToList();

                int totalItems = orderedMicroPosts.Count;
                int totalPages = totalItems > 0 ? (int)Math.Ceiling((double)totalItems / pageSize) : 1;
                
                // Ensure page number is within valid range
                if (pageNumber < 1)
                {
                    _logger.Warn($"Page number {pageNumber} is less than 1, setting to 1");
                    pageNumber = 1;
                }
                else if (totalItems > 0 && pageNumber > totalPages)
                {
                    _logger.Warn($"Page number {pageNumber} exceeds total pages {totalPages}, setting to {totalPages}");
                    pageNumber = totalPages;
                }

                _logger.Info($"Pagination: Total={totalItems}, PageSize={pageSize}, TotalPages={totalPages}, RequestedPage={pageNumber}");

                // Apply pagination
                microPostFeedViewModel.OnePageOfMicroPosts = orderedMicroPosts.ToPagedList(pageNumber, pageSize);

                _logger.Info($"SUCCESS: Built feed with {microPostFeedViewModel.MicroPostFeedItems.Count} total items, Page {microPostFeedViewModel.OnePageOfMicroPosts.PageNumber}/{microPostFeedViewModel.OnePageOfMicroPosts.PageCount}, {microPostFeedViewModel.OnePageOfMicroPosts.Count} items on current page");
                
                if (microPostFeedViewModel.OnePageOfMicroPosts.Count == 0 && totalItems > 0)
                {
                    _logger.Error($"WARNING: Pagination resulted in 0 items but total is {totalItems}!");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"EXCEPTION in BuildMicroPostFeedViewModelAsync for user {userId}: {ex.Message}");
                _logger.Error($"Exception type: {ex.GetType().FullName}");
                _logger.Error($"Stack trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    _logger.Error($"Inner exception: {ex.InnerException.Message}");
                }
                // Always ensure OnePageOfMicroPosts is set
                microPostFeedViewModel.OnePageOfMicroPosts = new List<MicroPostFeedItemViewModel>().ToPagedList(1, pageSize);
            }

            _logger.Info($"===== BuildMicroPostFeedViewModelAsync RETURNING ===== OnePageOfMicroPosts is null: {microPostFeedViewModel.OnePageOfMicroPosts == null}, Count: {microPostFeedViewModel.OnePageOfMicroPosts?.Count ?? -1}");
            
            return microPostFeedViewModel;
        }
    }
}
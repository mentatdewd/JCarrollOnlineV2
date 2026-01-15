using JCarrollOnlineV2.Entities;
using JCarrollOnlineV2.EntityFramework;
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
    public class UserStatsViewModelService : IUserStatsViewModelService
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly JCarrollOnlineV2DbContext _context;

        public UserStatsViewModelService(JCarrollOnlineV2DbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<UserStatsViewModel> BuildUserStatsViewModelAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException("User ID cannot be null or empty", nameof(userId));
            }

            _logger.Info($"Building user stats for user {userId}");

            var userStatsViewModel = new UserStatsViewModel
            {
                UserFollowers = new UserFollowersViewModel(),
                UsersFollowing = new UserFollowingViewModel()
            };

            try
            {
                // Load user with followers and following
                var user = await _context.ApplicationUser
                    .Include(u => u.Followers)
                    .Include(u => u.Following)
                    .AsNoTracking()
                    .SingleOrDefaultAsync(u => u.Id == userId)
                    .ConfigureAwait(false);

                if (user == null)
                {
                    _logger.Warn($"User {userId} not found");
                    return userStatsViewModel;
                }

                userStatsViewModel.User.InjectFrom(user);

                // Get all user IDs we need to query for micropost counts
                List<string> allUserIds = new List<string>();
                allUserIds.AddRange(user.Followers.Select(f => f.Id));
                allUserIds.AddRange(user.Following.Select(f => f.Id));

                // Get micropost counts for all users in ONE query to avoid multiple database calls
                _logger.Info($"Loading micropost counts for {allUserIds.Distinct().Count()} users");
                var microPostCounts = await _context.MicroPost
                    .AsNoTracking()
                    .Where(mp => allUserIds.Contains(mp.Author.Id))
                    .GroupBy(mp => mp.Author.Id)
                    .Select(g => new { UserId = g.Key, Count = g.Count() })
                    .ToDictionaryAsync(x => x.UserId, x => x.Count)
                    .ConfigureAwait(false);

                // Build followers view models
                _logger.Info($"Processing {user.Followers.Count} followers");
                foreach (ApplicationUser follower in user.Followers)
                {
                    var userItemViewModel = new UserItemViewModel(_logger);
                    userItemViewModel.InjectFrom(follower);
                    
                    // Get micro post count from the dictionary
                    userItemViewModel.MicroPostsAuthored = microPostCounts.ContainsKey(follower.Id) 
                        ? microPostCounts[follower.Id] 
                        : 0;

                    userStatsViewModel.UserFollowers.Users.Add(userItemViewModel);
                }

                // Build following view models
                _logger.Info($"Processing {user.Following.Count} following");
                foreach (ApplicationUser followedUser in user.Following)
                {
                    var userItemViewModel = new UserItemViewModel(_logger);
                    userItemViewModel.InjectFrom(followedUser);
                    
                    // Get micro post count from the dictionary
                    userItemViewModel.MicroPostsAuthored = microPostCounts.ContainsKey(followedUser.Id) 
                        ? microPostCounts[followedUser.Id] 
                        : 0;

                    userStatsViewModel.UsersFollowing.Users.Add(userItemViewModel);
                }

                _logger.Info($"Successfully built user stats");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error building user stats for user {userId}");
            }

            return userStatsViewModel;
        }
    }
}
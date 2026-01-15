using JCarrollOnlineV2.Entities;
using JCarrollOnlineV2.EntityFramework;
using JCarrollOnlineV2.ViewModels.Users;
using NLog;
using Omu.ValueInjecter;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace JCarrollOnlineV2.Services
{
    public class UserService : IUserService
    {
        private readonly JCarrollOnlineV2DbContext _context;
        private readonly ILogger _logger;

        public UserService(JCarrollOnlineV2DbContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<UserItemViewModel>> GetAllUsersAsync(string currentUserId = null)
        {
            IQueryable<ApplicationUser> query = _context.ApplicationUser
                .Include(u => u.Following)
                .Include(u => u.Followers)
                .Include(u => u.MicroPosts);

            if (!string.IsNullOrEmpty(currentUserId))
            {
                query = query.Where(u => u.Id != currentUserId);
            }

            List<ApplicationUser> users = await query.ToListAsync().ConfigureAwait(false);

            // Get current user's following and followers for status checks
            HashSet<string> currentUserFollowingIds = new HashSet<string>();
            HashSet<string> currentUserFollowerIds = new HashSet<string>();

            if (!string.IsNullOrEmpty(currentUserId))
            {
                var currentUser = await _context.ApplicationUser
                    .Include(u => u.Following)
                    .Include(u => u.Followers)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Id == currentUserId)
                    .ConfigureAwait(false);

                if (currentUser != null)
                {
                    currentUserFollowingIds = new HashSet<string>(currentUser.Following.Select(u => u.Id));
                    currentUserFollowerIds = new HashSet<string>(currentUser.Followers.Select(u => u.Id));
                }
            }

            List<UserItemViewModel> viewModels = new List<UserItemViewModel>();
            foreach (ApplicationUser user in users)
            {
                UserItemViewModel vm = new UserItemViewModel(_logger);
                vm.User.InjectFrom(user);
                vm.UserId = user.Id;
                vm.MicroPostsAuthored = user.MicroPosts.Count;
                
                // Set follower/following status
                vm.IsFollowing = currentUserFollowingIds.Contains(user.Id);
                vm.IsFollower = currentUserFollowerIds.Contains(user.Id);
                
                viewModels.Add(vm);
            }

            return viewModels;
        }

        public async Task<UserDetailViewModel> GetUserDetailsAsync(string userId, string currentUserId)
        {
            // Load the user with all relationships
            ApplicationUser user = await _context.ApplicationUser
                .Include(u => u.Following)
                .Include(u => u.Followers)
                .Include(u => u.MicroPosts)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId)
                .ConfigureAwait(false);

            if (user == null)
            {
                return null;
            }

            UserDetailViewModel viewModel = new UserDetailViewModel
            {
                UserInfoViewModel = new UserItemViewModel(_logger),
                UserStatsViewModel = new UserStatsViewModel
                {
                    UsersFollowing = new UserFollowingViewModel(),
                    UserFollowers = new UserFollowersViewModel()
                }
            };

            viewModel.User.InjectFrom(user);
            viewModel.UserInfoViewModel.User.InjectFrom(user);
            viewModel.UserInfoViewModel.MicroPostEmailNotifications = user.MicroPostEmailNotifications;
            viewModel.UserInfoViewModel.MicroPostSmsNotifications = user.MicroPostSmsNotifications;
            viewModel.UserInfoViewModel.UserId = user.Id;
            viewModel.UserInfoViewModel.MicroPostsAuthored = user.MicroPosts.Count;
            viewModel.UserStatsViewModel.User.InjectFrom(user);

            // Get all user IDs we need micropost counts for
            List<string> allUserIds = new List<string>();
            allUserIds.AddRange(user.Following.Select(u => u.Id));
            allUserIds.AddRange(user.Followers.Select(u => u.Id));

            // Get micropost counts for all users in ONE query
            var microPostCounts = await _context.MicroPost
                .AsNoTracking()
                .Where(mp => allUserIds.Contains(mp.Author.Id))
                .GroupBy(mp => mp.Author.Id)
                .Select(g => new { UserId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.UserId, x => x.Count)
                .ConfigureAwait(false);

            // Get current user's following/followers for badge indicators
            HashSet<string> currentUserFollowingIds = new HashSet<string>();
            HashSet<string> currentUserFollowerIds = new HashSet<string>();

            if (!string.IsNullOrEmpty(currentUserId) && currentUserId != userId)
            {
                var currentUser = await _context.ApplicationUser
                    .Include(u => u.Following)
                    .Include(u => u.Followers)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Id == currentUserId)
                    .ConfigureAwait(false);

                if (currentUser != null)
                {
                    currentUserFollowingIds = new HashSet<string>(currentUser.Following.Select(u => u.Id));
                    currentUserFollowerIds = new HashSet<string>(currentUser.Followers.Select(u => u.Id));
                }
            }

            // Map following users
            foreach (ApplicationUser following in user.Following)
            {
                UserItemViewModel vm = new UserItemViewModel(_logger);
                vm.User.InjectFrom(following);
                vm.UserId = following.Id;
                vm.MicroPostsAuthored = microPostCounts.ContainsKey(following.Id) 
                    ? microPostCounts[following.Id] 
                    : 0;
                
                // Set follower/following status for badges
                vm.IsFollowing = currentUserFollowingIds.Contains(following.Id);
                vm.IsFollower = currentUserFollowerIds.Contains(following.Id);
                
                viewModel.UserStatsViewModel.UsersFollowing.Users.Add(vm);
            }

            // Map followers
            foreach (ApplicationUser follower in user.Followers)
            {
                UserItemViewModel vm = new UserItemViewModel(_logger);
                vm.User.InjectFrom(follower);
                vm.UserId = follower.Id;
                vm.MicroPostsAuthored = microPostCounts.ContainsKey(follower.Id) 
                    ? microPostCounts[follower.Id] 
                    : 0;
                
                // Set follower/following status for badges
                vm.IsFollowing = currentUserFollowingIds.Contains(follower.Id);
                vm.IsFollower = currentUserFollowerIds.Contains(follower.Id);
                
                viewModel.UserStatsViewModel.UserFollowers.Users.Add(vm);
            }

            return viewModel;
        }

        public async Task<bool> FollowUserAsync(string currentUserId, string targetUserId)
        {
            ApplicationUser currentUser = await _context.ApplicationUser
                .Include(u => u.Following)
                .FirstOrDefaultAsync(u => u.Id == currentUserId)
                .ConfigureAwait(false);

            ApplicationUser targetUser = await _context.ApplicationUser
                .FirstOrDefaultAsync(u => u.Id == targetUserId)
                .ConfigureAwait(false);

            if (currentUser == null || targetUser == null || currentUser.Following.Any(u => u.Id == targetUserId))
            {
                return false;
            }

            currentUser.Following.Add(targetUser);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            return true;
        }

        public async Task<bool> UnfollowUserAsync(string currentUserId, string targetUserId)
        {
            ApplicationUser currentUser = await _context.ApplicationUser
                .Include(u => u.Following)
                .FirstOrDefaultAsync(u => u.Id == currentUserId)
                .ConfigureAwait(false);

            ApplicationUser targetUser = currentUser?.Following.FirstOrDefault(u => u.Id == targetUserId);

            if (currentUser == null || targetUser == null)
            {
                return false;
            }

            currentUser.Following.Remove(targetUser);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            return true;
        }

        public async Task<bool> UpdateUserSettingsAsync(string userId, bool emailNotifications, bool smsNotifications)
        {
            ApplicationUser user = await _context.ApplicationUser
                .FirstOrDefaultAsync(u => u.Id == userId)
                .ConfigureAwait(false);

            if (user == null)
            {
                return false;
            }

            user.MicroPostEmailNotifications = emailNotifications;
            // user.MicroPostSMSNotifications = smsNotifications;
            await _context.SaveChangesAsync().ConfigureAwait(false);
            return true;
        }
    }
}
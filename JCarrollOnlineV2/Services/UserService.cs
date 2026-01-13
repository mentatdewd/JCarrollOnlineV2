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

        public async Task<List<UserItemViewModel>> GetAllUsersAsync(string excludeUserId = null)
        {
            IQueryable<ApplicationUser> query = _context.ApplicationUser
                .Include(u => u.Following)
                .Include(u => u.Followers)
                .Include(u => u.MicroPosts);

            if (!string.IsNullOrEmpty(excludeUserId))
            {
                query = query.Where(u => u.Id != excludeUserId);
            }

            List<ApplicationUser> users = await query.ToListAsync().ConfigureAwait(false);

            List<UserItemViewModel> viewModels = new List<UserItemViewModel>();
            foreach (ApplicationUser user in users)
            {
                UserItemViewModel vm = new UserItemViewModel(_logger);
                vm.User.InjectFrom(user);
                vm.UserId = user.Id;
                vm.MicroPostsAuthored = user.MicroPosts.Count;
                viewModels.Add(vm);
            }

            return viewModels;
        }

        public async Task<UserDetailViewModel> GetUserDetailsAsync(string userId, string currentUserId)
        {
            ApplicationUser user = await _context.ApplicationUser
                .Include(u => u.Following)
                .Include(u => u.Followers)
                .Include(u => u.MicroPosts)
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
            viewModel.UserInfoViewModel.UserId = currentUserId;
            viewModel.UserStatsViewModel.User.InjectFrom(user);

            // Map following users
            foreach (ApplicationUser following in user.Following)
            {
                UserItemViewModel vm = new UserItemViewModel(_logger);
                vm.User.InjectFrom(following);
                vm.UserId = following.Id;
                vm.MicroPostsAuthored = following.MicroPosts.Count;
                viewModel.UserStatsViewModel.UsersFollowing.Users.Add(vm);
            }

            // Map followers
            foreach (ApplicationUser follower in user.Followers)
            {
                UserItemViewModel vm = new UserItemViewModel(_logger);
                vm.User.InjectFrom(follower);
                vm.UserId = follower.Id;
                vm.MicroPostsAuthored = follower.MicroPosts.Count;
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
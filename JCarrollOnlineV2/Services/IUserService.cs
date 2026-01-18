using JCarrollOnlineV2.Entities;
using JCarrollOnlineV2.ViewModels.Users;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace JCarrollOnlineV2.Services
{
    public interface IUserService
    {
        Task<List<UserItemViewModel>> GetAllUsersAsync(string excludeUserId = null);
        Task<UserDetailViewModel> GetUserDetailsAsync(string userId, string currentUserId);
        Task<bool> FollowUserAsync(string currentUserId, string targetUserId, CancellationToken cancellationToken = default);
        Task<bool> UnfollowUserAsync(string currentUserId, string targetUserId, CancellationToken cancellationToken = default);
        Task<bool> UpdateUserSettingsAsync(string userId, bool emailNotifications, bool smsNotifications, CancellationToken cancellationToken = default);
    }
}
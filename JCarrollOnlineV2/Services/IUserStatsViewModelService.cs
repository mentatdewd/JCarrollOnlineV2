using JCarrollOnlineV2.ViewModels.Users;
using System.Threading.Tasks;

namespace JCarrollOnlineV2.Services
{
    public interface IUserStatsViewModelService
    {
        /// <summary>
        /// Builds user statistics view model including followers and following
        /// </summary>
        /// <param name="userId">The user ID to build stats for</param>
        Task<UserStatsViewModel> BuildUserStatsViewModelAsync(string userId);
    }
}
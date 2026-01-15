using JCarrollOnlineV2.ViewModels.Home;
using System.Threading.Tasks;

namespace JCarrollOnlineV2.Services
{
    public interface IHomeViewModelService
    {
        /// <summary>
        /// Builds the complete home view model for authenticated users
        /// </summary>
        /// <param name="userId">The authenticated user ID</param>
        /// <param name="microPostPage">Current page for micro post pagination</param>
        Task<HomeViewModel> BuildAuthenticatedHomeViewModelAsync(string userId, int? microPostPage);

        /// <summary>
        /// Builds the home view model for anonymous users
        /// </summary>
        Task<HomeViewModel> BuildAnonymousHomeViewModelAsync();
    }
}
using JCarrollOnlineV2.ViewModels.MicroPosts;
using System.Threading.Tasks;

namespace JCarrollOnlineV2.Services
{
    public interface IMicroPostViewModelService
    {
        /// <summary>
        /// Builds a micro post feed view model for a specific user
        /// </summary>
        /// <param name="userId">The user ID to build the feed for</param>
        /// <param name="pageNumber">The page number for pagination</param>
        /// <param name="pageSize">Number of items per page</param>
        Task<MicroPostFeedViewModel> BuildMicroPostFeedViewModelAsync(string userId, int pageNumber, int pageSize);
    }
}
using JCarrollOnlineV2.ViewModels.Rss;
using System.Threading.Tasks;

namespace JCarrollOnlineV2.Services
{
    /// <summary>
    /// Service interface for RSS feed operations
    /// </summary>
    public interface IRssService
    {
        /// <summary>
        /// Asynchronously retrieves and processes RSS feed data
        /// </summary>
        /// <returns>A task that represents the asynchronous operation, containing the RSS feed view model</returns>
        Task<RssFeedViewModel> GetRssFeedAsync();
    }
}

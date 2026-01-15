using JCarrollOnlineV2.ViewModels.Blog;
using System.Threading.Tasks;

namespace JCarrollOnlineV2.Services
{
    public interface IBlogViewModelService
    {
        /// <summary>
        /// Builds a complete blog feed view model with all blog items and comments
        /// </summary>
        Task<BlogFeedViewModel> BuildBlogFeedViewModelAsync();
    }
}
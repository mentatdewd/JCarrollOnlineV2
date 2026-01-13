using JCarrollOnlineV2.Services;
using JCarrollOnlineV2.ViewModels.Rss;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JCarrollOnlineV2.Test.Services
{
    /// <summary>
    /// Mock RSS service for unit testing that returns empty RSS feed without making external calls
    /// </summary>
    public class MockRssService : IRssService
    {
        /// <summary>
        /// Returns an empty RSS feed view model for testing purposes
        /// </summary>
        /// <returns>A task that represents the asynchronous operation, containing an empty RSS feed view model</returns>
        public Task<RssFeedViewModel> GetRssFeedAsync()
        {
            RssFeedViewModel rssFeedViewModel = new RssFeedViewModel
            {
                RssFeedItems = new List<RssFeedItemViewModel>()
            };

            return Task.FromResult(rssFeedViewModel);
        }
    }
}

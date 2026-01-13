using JCarrollOnlineV2.ViewModels.Rss;
using NLog;
using Omu.ValueInjecter;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace JCarrollOnlineV2.Services
{
    /// <summary>
    /// Service for RSS feed operations
    /// </summary>
    public class RssService : IRssService
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Asynchronously retrieves and processes RSS feed data from MLB Mariners
        /// </summary>
        /// <returns>A task that represents the asynchronous operation, containing the RSS feed view model</returns>
        public async Task<RssFeedViewModel> GetRssFeedAsync()
        {
            _logger.Info("Obtaining rss data");
            Uri mlbUri = new Uri("https://www.mlb.com/mariners/feeds/news/rss.xml");
            TNX.RssReader.RssFeed rssFeed = await TNX.RssReader.RssHelper.ReadFeedAsync(mlbUri).ConfigureAwait(false);

            _logger.Info("Processing rss data");
            RssFeedViewModel rssFeedViewModel = new RssFeedViewModel
            {
                RssFeedItems = new List<RssFeedItemViewModel>()
            };

            foreach (TNX.RssReader.RssItem item in rssFeed.Items)
            {
                if (!string.IsNullOrWhiteSpace(item.Link))
                {
                    RssFeedItemViewModel rss = new RssFeedItemViewModel();

                    rss.InjectFrom(item);
                    rss.Link = new Uri(item.Link);
                    rss.UpdatedAt = DateTime.Now;
                    rssFeedViewModel.RssFeedItems.Add(rss);
                }
            }

            _logger.Info(string.Format(CultureInfo.InvariantCulture, "Processed {0} rss records", rssFeedViewModel.RssFeedItems.Count));

            return rssFeedViewModel;
        }
    }
}

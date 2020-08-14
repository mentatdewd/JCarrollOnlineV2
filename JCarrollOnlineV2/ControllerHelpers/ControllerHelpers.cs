using JCarrollOnlineV2.DataContexts;
using JCarrollOnlineV2.Entities;
using JCarrollOnlineV2.EntityFramework;
using JCarrollOnlineV2.ViewModels.Fora;
using JCarrollOnlineV2.ViewModels.Rss;
using JCarrollOnlineV2.ViewModels.Users;
using NLog;
using Omu.ValueInjecter;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace JCarrollOnlineV2
{
    public static class ControllerHelpers
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public static async Task<int> GetThreadPostCountAsync(int thread, IJCarrollOnlineV2Context data)
        {
            return data == null
                ? throw new ArgumentNullException(nameof(data))
                : await data.ForumThreadEntry.Where(i => i.RootId == thread).AsQueryable().CountAsync().ConfigureAwait(false);
        }

        public static async Task<int> GetThreadCountAsync(Forum forum, JCarrollOnlineV2DbContext data)
        {
            return data == null
                ? throw new ArgumentNullException(nameof(data))
                : await data.ForumThreadEntry.Where(i => i.Forum.Id == forum.Id && i.ParentId == null).CountAsync().ConfigureAwait(false);
        }

        public static async Task<DateTime> GetLastReplyAsync(int? rootId, IJCarrollOnlineV2Context data)
        {
            if (rootId != null)
            {
                if (data == null)
                {
                    throw new ArgumentNullException(nameof(data));
                }

                ThreadEntry fte = await data.ForumThreadEntry.Where(m => m.RootId == rootId).OrderByDescending(m => m.UpdatedAt).FirstOrDefaultAsync().ConfigureAwait(false);
                return fte.UpdatedAt;
            }
            else
            {
                return DateTime.Now;
            }
        }

        public static async Task<LastThreadViewModel> GetLatestThreadDataAsync(Forum forum, JCarrollOnlineV2DbContext data)
        {
            LastThreadViewModel lastThreadViewModel = new LastThreadViewModel();

            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            ThreadEntry forumThreadEntry = await data.ForumThreadEntry.Where(i => i.Forum.Id == forum.Id)
                .Include(i => i.Author)
                .OrderByDescending(i => i.UpdatedAt)
                .FirstOrDefaultAsync().ConfigureAwait(false);

            if (forumThreadEntry != null)
            {
                lastThreadViewModel.InjectFrom(forumThreadEntry);
                lastThreadViewModel.Author = new ApplicationUserViewModel();
                lastThreadViewModel.Author.InjectFrom(forumThreadEntry.Author);
                lastThreadViewModel.Forum = new ForaViewModel();
                lastThreadViewModel.Forum.InjectFrom(forumThreadEntry.Forum);

                bool rootNotFound = true;

                if (forumThreadEntry.ParentId != null)
                {
                    while (rootNotFound)
                    {
                        forumThreadEntry = await data.ForumThreadEntry.FindAsync(forumThreadEntry.ParentId).ConfigureAwait(false);
                        if (forumThreadEntry != null)
                        {
                            if (forumThreadEntry.ParentId == null)
                            {
                                rootNotFound = false;
                            }
                        }
                    }
                }

                lastThreadViewModel.PostRoot = forumThreadEntry.Id;

                return lastThreadViewModel;
            }

            return null;
        }

        public static async Task<RssFeedViewModel> UpdateRssAsync()
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
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
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static async Task<int> GetThreadPostCountAsync(int thread, IJCarrollOnlineV2Context data)
        {
            return await data.ForumThreadEntry.Where(i => i.RootId == thread).AsQueryable().CountAsync();
        }

        public static async Task<int> GetThreadCountAsync(Forum forum, JCarrollOnlineV2DbContext data)
        {
            return await data.ForumThreadEntry.Where(i => i.Forum.Id == forum.Id && i.ParentId == null).CountAsync();
        }

        public static async Task<DateTime> GetLastReplyAsync(int? rootId, IJCarrollOnlineV2Context data)
        {
            if (rootId != null)
            {
                ThreadEntry fte = await data.ForumThreadEntry.Where(m => m.RootId == rootId).OrderByDescending(m => m.UpdatedAt).FirstOrDefaultAsync();
                return fte.UpdatedAt;
            }
            else return DateTime.Now;
        }

        public static async Task<LastThreadViewModel> GetLatestThreadDataAsync(Forum forum, JCarrollOnlineV2DbContext data)
        {
            LastThreadViewModel lastThreadViewModel = new LastThreadViewModel();

            var forumThreadEntry = await data.ForumThreadEntry.Where(i => i.Forum.Id == forum.Id)
                .Include(i => i.Author)
                .OrderByDescending(i => i.UpdatedAt)
                .FirstOrDefaultAsync();

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
                        forumThreadEntry = await data.ForumThreadEntry.FindAsync(forumThreadEntry.ParentId);
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
            logger.Info("Obtaining rss data");
            TNX.RssReader.RssFeed rssFeed = await TNX.RssReader.RssHelper.ReadFeedAsync("http://m.mariners.mlb.com/partnerxml/gen/news/rss/sea.xml");

            logger.Info("Processing rss data");
            RssFeedViewModel rssFeedViewModel = new RssFeedViewModel
            {
                RssFeedItems = new List<RssFeedItemViewModel>()
            };

            foreach (var item in rssFeed.Items)
            {
                RssFeedItemViewModel rss = new RssFeedItemViewModel();

                rss.InjectFrom(item);
                rss.UpdatedAt = DateTime.Now;
                rssFeedViewModel.RssFeedItems.Add(rss);
            }

            logger.Info(string.Format(CultureInfo.InvariantCulture, "Processed {0} rss records", rssFeedViewModel.RssFeedItems.Count));

            return rssFeedViewModel;
        }
    }
}
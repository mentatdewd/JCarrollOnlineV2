using JCarrollOnlineV2.DataContexts;
using JCarrollOnlineV2.Entities;
using JCarrollOnlineV2.ViewModels;
using NLog;
using Omu.ValueInjecter;
using RazorEngine.Templating;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
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

        public static async Task<int> GetThreadCountAsync(Forum forum, IJCarrollOnlineV2Context data)
        {
            return await data.ForumThreadEntry.Where(i => i.Forum.Id == forum.Id && i.ParentId == null).CountAsync();
        }

        public static async Task<DateTime> GetLastReplyAsync(int? rootId, IJCarrollOnlineV2Context data)
        {
            if (rootId != null)
            {
                ForumThreadEntry fte = await data.ForumThreadEntry.Where(m => m.RootId == rootId).OrderByDescending(m => m.UpdatedAt).FirstOrDefaultAsync();
                return fte.UpdatedAt;
            }
            else return DateTime.Now;
        }

        public static async Task<LastThreadViewModel> GetLatestThreadDataAsync(Forum forum, IJCarrollOnlineV2Context data)
        {
            LastThreadViewModel ltVM = new LastThreadViewModel();

            var fte = await data.ForumThreadEntry.Where(i => i.Forum.Id == forum.Id)
                .Include(i => i.Author)
                .OrderByDescending(i => i.UpdatedAt)
                .FirstOrDefaultAsync();

            if (fte != null)
            {
                ltVM.InjectFrom(fte);
                ltVM.Author = new ApplicationUserViewModel();
                ltVM.Author.InjectFrom(fte.Author);
                ltVM.Forum = new ForaViewModel();
                ltVM.Forum.InjectFrom(fte.Forum);

                bool rootNotFound = true;
                if (fte.ParentId != null)
                    while (rootNotFound)
                    {
                        fte = await data.ForumThreadEntry.FindAsync(fte.ParentId);
                        if (fte != null)
                            if (fte.ParentId == null)
                                rootNotFound = false;
                    }
                ltVM.PostRoot = fte.Id;
                return ltVM;
            }
            return null;
        }

        public static async Task<RssFeedViewModel> UpdateRssAsync()
        {
            logger.Info("Obtaining rss data");
            TNX.RssReader.RssFeed rssFeed = await TNX.RssReader.RssHelper.ReadFeedAsync("http://m.mariners.mlb.com/partnerxml/gen/news/rss/sea.xml");

            logger.Info("Processing rss data");
            RssFeedViewModel rssFeedVM = new RssFeedViewModel();
            rssFeedVM.RssFeedItems = new List<RssFeedItemViewModel>();
            foreach (var item in rssFeed.Items)
            {
                RssFeedItemViewModel rss = new RssFeedItemViewModel();

                rss.InjectFrom(item);
                rss.UpdatedAt = DateTime.Now;
                rssFeedVM.RssFeedItems.Add(rss);
            }

            logger.Info(string.Format(CultureInfo.InvariantCulture, "Processed {0} rss records", rssFeedVM.RssFeedItems.Count));
            return rssFeedVM;
        }
    }

}
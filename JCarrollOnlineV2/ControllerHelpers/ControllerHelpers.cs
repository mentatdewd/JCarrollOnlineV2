using JCarrollOnlineV2.DataContexts;
using JCarrollOnlineV2.Entities;
using JCarrollOnlineV2.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using TNX;
using Omu.ValueInjecter;

namespace JCarrollOnlineV2.ControllerHelpers
{
    public static class Forums
    {
        public static int GetThreadCount(int ForumId)
        {
            {
                JCarrollOnlineV2Db db = new JCarrollOnlineV2Db();

                return db.ForumThreadEntries.Where(i => i.ForumId == ForumId && i.ParentId == null).Count();
            }
        }
        public static LastThreadViewModel GetLatestThreadData(int forumId)
        {
            JCarrollOnlineV2Db db = new JCarrollOnlineV2Db();
            LastThreadViewModel ltvm = new LastThreadViewModel();

            ForumThreadEntry fte = db.ForumThreadEntries.Where(i => i.ForumId == forumId).OrderByDescending(i => i.UpdatedAt).First();
            ltvm.UpdatedAt = fte.UpdatedAt;
            ltvm.Title = fte.Title;
            ltvm.PostNumber = fte.PostNumber;
            ltvm.Author = GetAuthor(fte.AuthorId);
            bool rootNotFound = true;
            if (fte.ParentId != null)
                while (rootNotFound)
                {
                    fte = db.ForumThreadEntries.Find(fte.ParentId);
                    if (fte.ParentId == null)
                        rootNotFound = false;
                }
            ltvm.PostRoot = fte.ForumThreadEntryId;
            return ltvm;
        }

        public static string GetAuthor(string authorId)
        {
            JCarrollOnlineV2Db db = new JCarrollOnlineV2Db();

            return db.Users.Find(authorId).UserName;
        }

        public static string GetParentAuthor(int? parentId)
        {
            if (parentId != null)
            {
                JCarrollOnlineV2Db db = new JCarrollOnlineV2Db();

                var parent = db.ForumThreadEntries.Find((int)parentId);
                return GetAuthor(parent.AuthorId);
            }
            else
                return null;
        }

        public static string GetForumName(int forumId)
        {
            JCarrollOnlineV2Db db = new JCarrollOnlineV2Db();

            return db.Forums.Find(forumId).Title;
        }

        public static async Task<RssFeedViewModel> UpdateRssAsync()
        {
            TNX.RssReader.RssFeed rssFeed = await TNX.RssReader.RssHelper.ReadFeedAsync("http://m.mariners.mlb.com/partnerxml/gen/news/rss/sea.xml");

            RssFeedViewModel rssFeedVM = new RssFeedViewModel();
            rssFeedVM.RssFeedItems = new List<RssFeedItemViewModel>();
            foreach (var item in rssFeed.Items)
            {
                RssFeedItemViewModel rss = new RssFeedItemViewModel();

                rss.InjectFrom(item);
                rss.UpdatedAt = DateTime.Now;
                rssFeedVM.RssFeedItems.Add(rss);
            }
            return rssFeedVM;
        }
    }
}
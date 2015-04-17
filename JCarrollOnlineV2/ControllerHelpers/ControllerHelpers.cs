using JCarrollOnlineV2.DataContexts;
using JCarrollOnlineV2.Entities;
using JCarrollOnlineV2.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Omu.ValueInjecter;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace JCarrollOnlineV2
{
    public static class ControllerHelpers
    {
        public static string GetAuthor(string authorId)
        {
            JCarrollOnlineV2Db db = new JCarrollOnlineV2Db();

            return db.Users.Find(authorId).UserName;
        }

        public static async Task<string> GetAuthorAsync(string authorId)
        {
            JCarrollOnlineV2Db db = new JCarrollOnlineV2Db();

            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            var user = await userManager.FindByIdAsync(authorId);
            return user.UserName;
        }

        public static int GetThreadPostCount(int thread)
        {
            JCarrollOnlineV2Db db = new JCarrollOnlineV2Db();

            return db.ForumThreadEntries.Where(i => i.RootId == thread).AsQueryable().Count();
        }

        public static async Task<int> GetThreadPostCountAsync(int thread)
        {
            JCarrollOnlineV2Db db = new JCarrollOnlineV2Db();

            return await db.ForumThreadEntries.Where(i => i.RootId == thread).AsQueryable().CountAsync();
        }

        public static int GetThreadCount(int forumId)
        {
            JCarrollOnlineV2Db db = new JCarrollOnlineV2Db();

            return db.ForumThreadEntries.Where(i => i.ForumId == forumId && i.ParentId == null).Count();
        }

        public static async Task<int> GetThreadCountAsync(int forumId)
        {
                JCarrollOnlineV2Db db = new JCarrollOnlineV2Db();

                return await db.ForumThreadEntries.Where(i => i.ForumId == forumId && i.ParentId == null).CountAsync();
        }

        public static int GetAuthorPostCount(string authorId)
        {
            JCarrollOnlineV2Db db = new JCarrollOnlineV2Db();

            return db.ForumThreadEntries.Where(i => i.AuthorId == authorId).Count();
        }

        public static async Task<int> GetAuthorPostCountAsync(string authorId)
        {
            JCarrollOnlineV2Db db = new JCarrollOnlineV2Db();

            return await db.ForumThreadEntries.Where(i => i.AuthorId == authorId).CountAsync();
        }

        public static DateTime GetLastReply(int? rootId)
        {
            if (rootId != null)
            {
                JCarrollOnlineV2Db db = new JCarrollOnlineV2Db();

                ForumThreadEntry fte = db.ForumThreadEntries.Where(m => m.RootId == rootId).OrderByDescending(m => m.UpdatedAt).FirstOrDefault();
                return fte.UpdatedAt;
            }
            else return DateTime.Now;
        }

        public static async Task<DateTime> GetLastReplyAsync(int? rootId)
        {
            if (rootId != null)
            {
                JCarrollOnlineV2Db db = new JCarrollOnlineV2Db();

                ForumThreadEntry fte = await db.ForumThreadEntries.Where(m => m.RootId == rootId).OrderByDescending(m => m.UpdatedAt).FirstOrDefaultAsync();
                return fte.UpdatedAt;
            }
            else return DateTime.Now;
        }

        public static int GetParentPostNumber(int? parentId)
        {
            if (parentId != null)
            {
                JCarrollOnlineV2Db db = new JCarrollOnlineV2Db();

                return db.ForumThreadEntries.Find(parentId).PostNumber;
            }
            return 1;
        }

        public static async Task<int> GetParentPostNumberAsync(int? parentId)
        {
            if (parentId != null)
            {
                JCarrollOnlineV2Db db = new JCarrollOnlineV2Db();

                var parent = await db.ForumThreadEntries.FindAsync(parentId);

                return parent.PostNumber;
            }
            return 1;
        }

        public static LastThreadViewModel GetLatestThreadData(int forumId)
        {
            JCarrollOnlineV2Db db = new JCarrollOnlineV2Db();
            LastThreadViewModel ltvm = new LastThreadViewModel();

            ForumThreadEntry fte = db.ForumThreadEntries.Where(i => i.ForumId == forumId).OrderByDescending(i => i.UpdatedAt).FirstOrDefault();

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

        public static async Task<LastThreadViewModel> GetLatestThreadDataAsync(int forumId)
        {
            JCarrollOnlineV2Db db = new JCarrollOnlineV2Db();
            LastThreadViewModel ltvm = new LastThreadViewModel();

            ForumThreadEntry fte = await db.ForumThreadEntries.Where(i => i.ForumId == forumId).OrderByDescending(i => i.UpdatedAt).FirstOrDefaultAsync();

            ltvm.UpdatedAt = fte.UpdatedAt;
            ltvm.Title = fte.Title;
            ltvm.PostNumber = fte.PostNumber;
            ltvm.Author = GetAuthor(fte.AuthorId);
            ltvm.ForumId = fte.ForumId;
            bool rootNotFound = true;
            if (fte.ParentId != null)
                while (rootNotFound)
                {
                    fte = await db.ForumThreadEntries.FindAsync(fte.ParentId);
                    if (fte.ParentId == null)
                        rootNotFound = false;
                }
            ltvm.PostRoot = fte.ForumThreadEntryId;
            return ltvm;
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

        public static async Task<string> GetParentAuthorAsync(int? parentId)
        {
            if (parentId != null)
            {
                JCarrollOnlineV2Db db = new JCarrollOnlineV2Db();

                var parent = await db.ForumThreadEntries.FindAsync((int)parentId);
                return await GetAuthorAsync(parent.AuthorId);
            }
            else
                return null;
        }

        public static string GetForumName(int forumId)
        {
            JCarrollOnlineV2Db db = new JCarrollOnlineV2Db();

            return db.Forums.Find(forumId).Title;
        }

        public static async Task<string> GetForumNameAsync(int forumId)
        {
            JCarrollOnlineV2Db db = new JCarrollOnlineV2Db();

            var forumName = await db.Forums.FindAsync(forumId);
            return forumName.Title;
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
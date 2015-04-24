using JCarrollOnlineV2.DataContexts;
using JCarrollOnlineV2.Entities;
using JCarrollOnlineV2.ViewModels;
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
        //public static ApplicationUser GetAuthor(string authorId, IContext data)
        //{
        //    return data.Users.Find(authorId).UserName;
        //}

        //public static async Task<string> GetAuthorAsync(string authorId, IContext data)
        //{
        //    var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(data as DbContext));
        //    var user = await userManager.FindByIdAsync(authorId);
        //    return user.UserName;
        //}

        public static int GetThreadPostCount(int thread, IContext data)
        {
            return data.ForumThreadEntries.Where(i => i.RootId == thread).AsQueryable().Count();
        }

        public static async Task<int> GetThreadPostCountAsync(int thread, IContext data)
        {
            return await data.ForumThreadEntries.Where(i => i.RootId == thread).AsQueryable().CountAsync();
        }

        public static int GetThreadCount(Forum forum, IContext data)
        {
            //return data.ForumThreadEntries.Where(i => i.ForumId == forumId && i.ParentId == null).Count();
            return data.ForumThreadEntries.Where(i => i.Forum.Id == forum.Id && i.ParentId == null).Count();
        }

        public static async Task<int> GetThreadCountAsync(Forum forum, IContext data)
        {
                //return await data.ForumThreadEntries.Where(i => i.ForumId == forumId && i.ParentId == null).CountAsync();
            return await data.ForumThreadEntries.Where(i => i.Forum.Id == forum.Id && i.ParentId == null).CountAsync();
        }

        public static int GetAuthorPostCount(ApplicationUser author, IContext data)
        {
            //return data.ForumThreadEntries.Where(i => i.AuthorId == authorId).Count();
            return data.ForumThreadEntries.Where(i => i.Author.Id == author.Id).Count();
        }

        public static async Task<int> GetAuthorPostCountAsync(ApplicationUser author, IContext data)
        {
            //return await data.ForumThreadEntries.Where(i => i.AuthorId == authorId).CountAsync();
            return await data.ForumThreadEntries.Where(i => i.Author.Id == author.Id).CountAsync();
        }

        public static DateTime GetLastReply(int? rootId, IContext data)
        {
            if (rootId != null)
            {
                ForumThreadEntry fte = data.ForumThreadEntries.Where(m => m.RootId == rootId).OrderByDescending(m => m.UpdatedAt).FirstOrDefault();
                return fte.UpdatedAt;
            }
            else return DateTime.Now;
        }

        public static async Task<DateTime> GetLastReplyAsync(int? rootId, IContext data)
        {
            if (rootId != null)
            {
                ForumThreadEntry fte = await data.ForumThreadEntries.Where(m => m.RootId == rootId).OrderByDescending(m => m.UpdatedAt).FirstOrDefaultAsync();
                return fte.UpdatedAt;
            }
            else return DateTime.Now;
        }

        public static int GetParentPostNumber(int? parentId, IContext data)
        {
            if (parentId != null)
            {
                return data.ForumThreadEntries.Find(parentId).PostNumber;
            }
            return 1;
        }

        public static async Task<int> GetParentPostNumberAsync(int? parentId, IContext data)
        {
            if (parentId != null)
            {
                var parent = await data.ForumThreadEntries.FindAsync(parentId);

                return parent.PostNumber;
            }
            return 1;
        }

        public static LastThreadViewModel GetLatestThreadData(Forum forum, IContext data)
        {
            LastThreadViewModel ltvm = new LastThreadViewModel();

            var fte = data.ForumThreadEntries.Where(i => i.Forum.Id == forum.Id)
                .Include(i => i.Author)
                .OrderByDescending(i => i.UpdatedAt)
                .FirstOrDefault();

            ltvm.InjectFrom(fte);
            ltvm.Author = new ApplicationUserViewModel();
            ltvm.Author.InjectFrom(fte.Author);

            bool rootNotFound = true;
            if (fte.ParentId != null)
                while (rootNotFound)
                {
                    fte = data.ForumThreadEntries.Find(fte.ParentId);
                    if (fte.ParentId == null)
                        rootNotFound = false;
                }
            ltvm.PostRoot = fte.Id;
            return ltvm;
        }

        public static async Task<LastThreadViewModel> GetLatestThreadDataAsync(Forum forum, IContext data)
        {
            LastThreadViewModel ltvm = new LastThreadViewModel();

            var fte = await data.ForumThreadEntries.Where(i => i.Forum.Id == forum.Id)
                .Include(i => i.Author)
                .OrderByDescending(i => i.UpdatedAt)
                .FirstOrDefaultAsync();

            ltvm.InjectFrom(fte);
            ltvm.Author = new ApplicationUserViewModel();
            ltvm.Author.InjectFrom(fte.Author);
            ltvm.Forum = new ForaViewModel();
            ltvm.Forum.InjectFrom(fte.Forum);

            bool rootNotFound = true;
            if (fte.ParentId != null)
                while (rootNotFound)
                {
                    fte = await data.ForumThreadEntries.FindAsync(fte.ParentId);
                    if (fte.ParentId == null)
                        rootNotFound = false;
                }
            ltvm.PostRoot = fte.Id;
            return ltvm;
        }

        public static ApplicationUser GetParentAuthor(int? parentId, IContext data)
        {
            if (parentId != null)
            {
                var parent = data.ForumThreadEntries.Find((int)parentId);
                return parent.Author;
            }
            else
                return null;
        }

        public static async Task<ApplicationUser> GetParentAuthorAsync(int? parentId, IContext data)
        {
            if (parentId != null)
            {
                var parent = await data.ForumThreadEntries.FindAsync((int)parentId);
                return parent.Author;
            }
            else
                return null;
        }

        //public static string GetForumName(int forumId, IContext data)
        //{
        //    return data.Forums.Find(forumId).Title;
        //}

        //public static async Task<string> GetForumNameAsync(int forumId, IContext data)
        //{
        //    var forumName = await data.Forums.FindAsync(forumId);
        //    return forumName.Title;
        //}

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

        public static bool Following(ApplicationUser user, IContext data)
        {
            var result = data.Users
                .Where(m => m.Following == user);

            return result == null ? false : true;
        }
    }

}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using JCarrollOnlineV2.ViewModels;
using JCarrollOnlineV2.DataContexts;

namespace JCarrollOnlineV2.CustomHtmlHelpers
{
    public static class UserInfoHtmlHelper
    {
        public static int GetPostCount(string author)
        {
            JCarrollOnlineV2Db db = new JCarrollOnlineV2Db();
            //var author = GetAuthor(user);

            //var count = db.Users.Where(i => i.Id == author).Single().ForumThreadEntries.Count;
            int count = db.ForumThreadEntries.Where(i => i.AuthorId == author).Count();
            return count;
        }
        public static int GetParentPostNumber(int ? parentId)
        {
            JCarrollOnlineV2Db db = new JCarrollOnlineV2Db();

            int parentPostNumber = db.ForumThreadEntries.Find((int)parentId).PostNumber;
            return parentPostNumber;
        }
        public static string GetAuthor(string authorId)
        {
            JCarrollOnlineV2Db db = new JCarrollOnlineV2Db();
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            var user = userManager.FindById(authorId);
            return user.UserName;
        }
    }
}
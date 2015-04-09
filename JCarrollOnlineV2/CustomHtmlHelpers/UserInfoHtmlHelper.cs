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
        public static string GetAuthor(string authorId)
        {
            JCarrollOnlineV2Db db = new JCarrollOnlineV2Db();
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            var user = userManager.FindById(authorId);
            return user.UserName;
        }
        public static int GetPostCount(string user)
        {
            int postCount = 0;
            return postCount;
        }
    }
}
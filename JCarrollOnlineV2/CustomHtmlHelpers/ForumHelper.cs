using JCarrollOnlineV2.DataContexts;
using JCarrollOnlineV2.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JCarrollOnlineV2.CustomHtmlHelpers
{

    public static class ForumHelper
    {
        public static string GetLatestThreadTitle(int forumId)
        {
            JCarrollOnlineV2Db db = new JCarrollOnlineV2Db();

            return  db.ForumThreadEntries.Where(i => i.ForumId == forumId).OrderBy(i => i.UpdatedAt).First().Title;
        }
        public static int GetParentPostNumber(int? forumThreadEntryId)
        {
            JCarrollOnlineV2Db db = new JCarrollOnlineV2Db();

            if (forumThreadEntryId != null)
                return db.ForumThreadEntries.Where(i => i.ForumThreadEntryId == forumThreadEntryId).FirstOrDefault().PostNumber;
            else
                return 1;
        }
        public static DateTime GetLatestThreadPostDate(int? rootId)
        {
            if (rootId != null)
            {
                JCarrollOnlineV2Db db = new JCarrollOnlineV2Db();

                return db.ForumThreadEntries.Where(m => m.RootId == rootId).OrderByDescending(m => m.UpdatedAt).FirstOrDefault().UpdatedAt;
            }
            else return DateTime.Now;
        }
    }
}
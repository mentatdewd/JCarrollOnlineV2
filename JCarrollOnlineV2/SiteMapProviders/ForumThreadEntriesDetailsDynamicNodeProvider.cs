using JCarrollOnlineV2.EntityFramework;
using MvcSiteMapProvider;
using System.Collections.Generic;

namespace JCarrollOnlineV2.SiteMapProviders
{
    public class ForumThreadEntriesDetailsDynamicNodeProvider : DynamicNodeProviderBase
    {
        public override IEnumerable<DynamicNode> GetDynamicNodeCollection(ISiteMapNode node)
        {
            using (var jCarrollOnlineV2Db = new JCarrollOnlineV2DbContext())
            {
                // Create a node for each album 
                foreach (var forumThreadEntry in jCarrollOnlineV2Db.ForumThreadEntry)
                {
                    DynamicNode dynamicNode = new DynamicNode();
                    dynamicNode.Title = forumThreadEntry.Title;
                    dynamicNode.ParentKey = "Forum_" + forumThreadEntry.Forum.Title;
                    dynamicNode.RouteValues.Add("id", forumThreadEntry.Id);
                    dynamicNode.RouteValues.Add("forumId", forumThreadEntry.Forum.Id);
                    yield return dynamicNode;
                }
            }
        }
    }
}
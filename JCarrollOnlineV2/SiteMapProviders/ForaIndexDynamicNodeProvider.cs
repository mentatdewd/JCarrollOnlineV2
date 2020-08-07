using JCarrollOnlineV2.EntityFramework;
using MvcSiteMapProvider;
using System.Collections.Generic;

namespace JCarrollOnlineV2.SiteMapProviders
{
    public class ForaIndexDynamicNodeProvider : DynamicNodeProviderBase
    {
        public override IEnumerable<DynamicNode> GetDynamicNodeCollection(ISiteMapNode node)
        {
            using (JCarrollOnlineV2DbContext jCarrollOnlineV2Db = new JCarrollOnlineV2DbContext())
            {
                // Create a node for each album 
                foreach (Entities.Forum forum in jCarrollOnlineV2Db.Forum)
                {
                    DynamicNode dynamicNode = new DynamicNode();
                    dynamicNode.Title = forum.Title;
                    dynamicNode.ParentKey = "Home";
                    yield return dynamicNode;
                }
            }
        }
    }
}
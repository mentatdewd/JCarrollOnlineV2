using JCarrollOnlineV2.EntityFramework;
using MvcSiteMapProvider;
using System.Collections.Generic;

namespace JCarrollOnlineV2.SiteMapProviders
{
    public class ForaIndexDynamicNodeProvider : DynamicNodeProviderBase
    {
        public override IEnumerable<DynamicNode> GetDynamicNodeCollection(ISiteMapNode node)
        {
            using (var jCarrollOnlineV2Db = new JCarrollOnlineV2DbContext())
            {
                // Create a node for each album 
                foreach (var forum in jCarrollOnlineV2Db.Forum)
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
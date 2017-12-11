using JCarrollOnlineV2.ViewModels.HierarchyNode;
using System.Collections.Generic;

namespace JCarrollOnlineV2.ViewModels.ForumThreadEntries
{
    public class ThreadEntryDetailItemsViewModel : ThreadEntriesViewModelBase
    {
        public int NumberOfReplies { get; set; }

        public IEnumerable<HierarchyNodesViewModel<ThreadEntryDetailsItemViewModel>> ForumThreadEntries { get; set; }
    }
}

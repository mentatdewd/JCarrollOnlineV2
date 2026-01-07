using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace JCarrollOnlineV2.ViewModels.Home
{
    public class LatestForumThreadsViewModel : ViewModelBase
    {
        public IList<LatestForumThreadItemViewModel> LatestForumThreads { get; set; } = new Collection<LatestForumThreadItemViewModel>();

        public LatestForumThreadsViewModel()
        {
            LatestForumThreads = new List<LatestForumThreadItemViewModel>();
        }
    }
}

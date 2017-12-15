using System.Collections.ObjectModel;

namespace JCarrollOnlineV2.ViewModels
{
    public class LatestForumThreadsViewModel : ViewModelBase
    {
        private Collection<LatestForumThreadItemViewModel> _latestForumThreads = new Collection<LatestForumThreadItemViewModel>();
        public Collection<LatestForumThreadItemViewModel> LatestForumThreads { get { return _latestForumThreads; } private set { _latestForumThreads = value; } }
    }
}

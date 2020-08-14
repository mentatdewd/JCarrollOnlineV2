using System.Collections.ObjectModel;

namespace JCarrollOnlineV2.ViewModels
{
    public class LatestForumThreadsViewModel : ViewModelBase
    {
        public Collection<LatestForumThreadItemViewModel> LatestForumThreads { get; private set; } = new Collection<LatestForumThreadItemViewModel>();
    }
}

using JCarrollOnlineV2.ViewModels.Fora;

namespace JCarrollOnlineV2.ViewModels.ForumThreadEntries
{
    public class ThreadEntryDetailsViewModel : ThreadEntriesViewModelBase
    {
        private ThreadEntryDetailItemsViewModel _forumThreadEntryDetailItems = new ThreadEntryDetailItemsViewModel();
        private ForaDetailsViewModel _foraDetailsViewModel = new ForaDetailsViewModel();

        public ThreadEntryDetailItemsViewModel ForumThreadEntryDetailItems => _forumThreadEntryDetailItems;

        public ForaDetailsViewModel Forum => _foraDetailsViewModel;
        public int Replies { get; set; }
    }
}

using JCarrollOnlineV2.ViewModels.Fora;

namespace JCarrollOnlineV2.ViewModels.ForumThreadEntries
{
    public class ThreadEntryDetailsViewModel : ThreadEntriesViewModelBase
    {
        private ThreadEntryDetailItemsViewModel _forumThreadEntryDetailItems = new ThreadEntryDetailItemsViewModel();
        private ForaDetailsViewModel _foraDetailsViewModel = new ForaDetailsViewModel();

        public ThreadEntryDetailItemsViewModel ForumThreadEntryDetailItems { get { return _forumThreadEntryDetailItems; } }

        public ForaDetailsViewModel Forum { get { return _foraDetailsViewModel; } }
        public int Replies { get; set; }
    }
}

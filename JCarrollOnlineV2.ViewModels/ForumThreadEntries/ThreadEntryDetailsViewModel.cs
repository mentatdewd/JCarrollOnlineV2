using JCarrollOnlineV2.ViewModels.Fora;

namespace JCarrollOnlineV2.ViewModels.ForumThreadEntries
{
    public class ThreadEntryDetailsViewModel : ThreadEntriesViewModelBase
    {
        public ThreadEntryDetailItemsViewModel ForumThreadEntryDetailItems { get; set; }

        public ForaDetailsViewModel Forum { get; set; }
        public int Replies { get; set; }
    }
}

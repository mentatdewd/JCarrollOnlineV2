using System.ComponentModel.DataAnnotations;

namespace JCarrollOnlineV2.ViewModels.ForumThreadEntries
{
    public class ThreadEntriesViewModelBase : ViewModelBase
    {
        [Required]
        public int Id { get; set; }

    }
}

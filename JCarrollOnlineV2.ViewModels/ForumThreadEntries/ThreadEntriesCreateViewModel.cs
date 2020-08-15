using JCarrollOnlineV2.ViewModels.Users;
using System.ComponentModel.DataAnnotations;

namespace JCarrollOnlineV2.ViewModels.ForumThreadEntries
{
    public class ThreadEntriesCreateViewModel : ThreadEntriesViewModelBase
    {
        public int ParentPostNumber { get; set; }
        public int? ParentId { get; set; }
        public int? RootId { get; set; }
        public int ForumId { get; set; }

        [Display(Name = "Title")]
        [Required]
        public string Title { get; set; }

        [Display(Name = "Content")]
        [DataType(DataType.MultilineText)]
        public string Content { get; set; }

        public ApplicationUserViewModel Author { get; set; }
    }
}

using JCarrollOnlineV2.ViewModels.Fora;
using JCarrollOnlineV2.ViewModels.Users;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace JCarrollOnlineV2.ViewModels.ForumThreadEntries
{
    public class ThreadEntryDetailsItemViewModel : ThreadEntriesViewModelBase
    {
        public ThreadEntryDetailsItemViewModel()
        {
            Author = new ApplicationUserViewModel();
            Forum = new ForaViewModel();
        }

        public int? ParentId { get; set; }
        public int? RootId { get; set; }
        public int? ParentPostNumber { get; set; }
        public ForaViewModel Forum { get; set; }

        [Display(Name = "Parent Author")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string ParentAuthor { get; set; }

        [Display(Name = "Post Count")]
        public int PostCount { get; set; }

        [Display(Name = "Author")]
        public ApplicationUserViewModel Author { get; set; }

        [Display(Name = "Content")]
        public string Content { get; set; }

        [Display(Name = "Title")]
        public string Title { get; set; }

        [Display(Name = "Created On")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Updated On")]
        public DateTime UpdatedAt { get; set; }

        [Display(Name = "Post Number")]
        public int PostNumber { get; set; }

        public bool Locked { get; set; }
    }
}

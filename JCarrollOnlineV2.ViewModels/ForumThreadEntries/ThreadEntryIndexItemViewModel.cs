using JCarrollOnlineV2.ViewModels.Fora;
using JCarrollOnlineV2.ViewModels.Users;
using System;
using System.ComponentModel.DataAnnotations;

namespace JCarrollOnlineV2.ViewModels.ForumThreadEntries
{
    public class ThreadEntryIndexItemViewModel : ThreadEntriesViewModelBase
    {
        private ApplicationUserViewModel _author = new ApplicationUserViewModel();
        private ForaViewModel _forum = new ForaViewModel();

        public ForaViewModel Forum => _forum;

        [Display(Name = "Replies")]
        public int Replies { get; set; }

        [Display(Name = "Last Reply")]
        public DateTime LastReply { get; set; }

        [Display(Name = "Recs")]
        public int Recs { get; set; }

        [Display(Name = "Views")]
        public int Views { get; set; }

        [Display(Name = "Author")]
        public ApplicationUserViewModel Author => _author;

        [Display(Name = "Title")]
        public string Title { get; set; }

        [Display(Name = "Created On")]
        public DateTime CreatedAt { get; set; }
    }
}

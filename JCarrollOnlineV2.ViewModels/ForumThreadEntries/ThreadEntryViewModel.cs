using JCarrollOnlineV2.ViewModels.Fora;
using JCarrollOnlineV2.ViewModels.Users;
using System;
using System.ComponentModel.DataAnnotations;

namespace JCarrollOnlineV2.ViewModels.ForumThreadEntries
{
    public class ThreadEntryViewModel : ViewModelBase
    {
        [DataType(DataType.Text)]
        [StringLength(256)]
        public string Title { get; set; }

        [DataType(DataType.MultilineText)]
        public string Content { get; set; }

        public bool Locked { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } //           :null => false

        [DataType(DataType.DateTime)]
        public DateTime UpdatedAt { get; set; } //          :null => false

        public int PostNumber { get; set; }

        public int? ParentId { get; set; }

        public int? RootId { get; set; }

        public ApplicationUserViewModel Author { get; set; }
        public ForaViewModel Forum { get; set; }
    }
}

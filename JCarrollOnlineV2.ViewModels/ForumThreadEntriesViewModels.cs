using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace JCarrollOnlineV2.ViewModels
{
    public class ForumThreadEntriesViewModelBase : ViewModelBase
    {
        [Required]
        public int Id { get; set; }

    }

    public class ForumThreadEntryViewModel : ViewModelBase
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

    public class ForumThreadEntryIndexViewModel : ForumThreadEntriesViewModelBase
    {
        public List<ForumThreadEntryIndexItemViewModel> ForumThreadEntryIndex { get; set; }

        public ForaViewModel Forum { get; set; }

        [DataType(DataType.Text)]
        [StringLength(256)]
        [Display(Name = "Title")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string Title { get; set; }

        [DataType(DataType.MultilineText)]
        public string Content { get; set; }
    }

    public class ForumThreadEntryIndexItemViewModel : ForumThreadEntriesViewModelBase
    {
        public ForaViewModel Forum { get; set; }

        [Display(Name = "Replies")]
        public int Replies { get; set; }

        [Display(Name = "Last Reply")]
        public DateTime LastReply { get; set; }

        [Display(Name = "Recs")]
        public int Recs { get; set; }

        [Display(Name = "Views")]
        public int Views { get; set; }

        [Display(Name = "Author")]
        public ApplicationUserViewModel Author { get; set; }

        [Display(Name = "Title")]
        public string Title { get; set; }

        [Display(Name = "Created On")]
        public DateTime CreatedAt { get; set; }
    }

    public class ForumThreadEntryDetailsItemViewModel : ForumThreadEntriesViewModelBase
    {
        public int? ParentId { get; set; }
        public int? RootId { get; set; }
        public int? ParentPostNumber { get; set; }
        public ForaViewModel Forum { get; set; }

        [Display(Name = "Parent Author")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string ParentAuthor { get; set; }

        [Display(Name="Post Count")]
        public int PostCount { get; set; }

        [Display(Name="Author")]
        public ApplicationUserViewModel Author { get; set; }

        [Display(Name="Content")]
        public string Content { get; set; }

        [Display(Name = "Title")]
        public string Title { get; set; }

        [Display(Name = "Created On")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Updated On")]
        public DateTime UpdatedAt { get; set; }

        [Display(Name = "Post Number")]
        public int PostNumber { get; set; }
    }

    public class ForumThreadEntryDetailsViewModel : ForumThreadEntriesViewModelBase
    {
        public ForumThreadEntryDetailItemsViewModel ForumThreadEntryDetailItems { get; set; }

        public ForaDetailsViewModel Forum { get; set; }
        public int Replies { get; set; }
    }

    public class ForumThreadEntryDetailItemsViewModel : ForumThreadEntriesViewModelBase
    {
        public int NumberOfReplies { get; set; }

        public IEnumerable<HierarchyNodesViewModel<ForumThreadEntryDetailsItemViewModel>> ForumThreadEntries { get; set; }
    }

    public class ForumThreadEntriesCreateViewModel : ForumThreadEntriesViewModelBase
    {
        public int ParentPostNumber { get; set; }
        public int? ParentId { get; set; }
        public int? RootId { get; set; }
        public int ForumId { get; set; }

        [Display(Name = "Title")]
        public string Title { get; set; }

        [Display(Name = "Content")]
        [DataType(DataType.MultilineText)]
        public string Content { get; set; }

        [Display(Name = "Author")]
        public ApplicationUserViewModel Author { get; set; }
    }

    public class ForumThreadEntriesEditViewModel : ForumThreadEntriesViewModelBase
    {
        public int? ParentId { get; set; }
        public int? RootId { get; set; }
        public int ForumId { get; set; }
        public string AuthorId { get; set; }

        [Display(Name = "Post Number")]
        public int PostNumber { get; set; }

        [Display(Name = "Locked")]
        public bool Locked { get; set; }

        [Display(Name = "Created On")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Author")]
        public ApplicationUserViewModel Author { get; set; }

        [Display(Name = "Title")]
        public string Title { get; set; }

        [Display(Name = "Content")]
        public string Content { get; set; }
    }
}

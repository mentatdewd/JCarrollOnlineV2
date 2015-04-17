using JCarrollOnlineV2.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace JCarrollOnlineV2.ViewModels
{
    public class ForumThreadEntriesViewModelBase : ViewModelBase
    {
        [Required]
        public int ForumId { get; set; }

        [Required]
        public int ForumThreadEntryId { get; set; }

        public int? RootId { get; set; }

        [DataType(DataType.Text)]
        [StringLength(256)]
        [Display(Name = "Title")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string Title { get; set; }

        [DataType(DataType.MultilineText)]
        public string Content { get; set; }


        [Display(Name = "Author")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string Author { get; set; }

        public bool Locked { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Created at")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public DateTime CreatedAt { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime UpdatedAt { get; set; } //          :null => false

        public int PostNumber { get; set; }

        public int? ParentId { get; set; }

        public string AuthorId { get; set; }
    }

    public class ForumThreadEntryIndexViewModel : ForumThreadEntriesViewModelBase
    {
        public string ForumTitle { get; set; }
        public List<ForumThreadEntryIndexItemViewModel> ForumThreadIndexEntries { get; set; }
    }

    public class ForumThreadEntryIndexItemViewModel : ForumThreadEntriesViewModelBase
    {

        [Display(Name = "Replies")]
        public int Replies { get; set; }


        [Display(Name = "Last Reply")]
        public DateTime LastReply { get; set; }

        [Display(Name = "Recs")]
        public int Recs { get; set; }

        [Display(Name = "Views")]
        public int Views { get; set; }
    }

    public class ForumThreadEntryDetailsItemViewModel : ForumThreadEntriesViewModelBase
    {
        [Display(Name = "Parent Author")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string ParentAuthor { get; set; }

        public int PostCount { get; set; }

        public int ParentPostNumber { get; set; }
    }

    public class ForumThreadEntryDetailsViewModel : ForumThreadEntriesViewModelBase
    {
        public string ForumTitle { get; set; }
        public ForumThreadEntryDetailItemsViewModel ForumThreadEntryDetailItems { get; set; }
        public ForumThreadEntryTOCItemsViewModel ForumThreadEntryTOCItems { get; set; }
    }

    public class ForumThreadEntryDetailItemsViewModel : ForumThreadEntriesViewModelBase
    {
        public IEnumerable<HierarchyNode<ForumThreadEntryDetailsItemViewModel>> ForumThreadEntries { get; set; }
    }

    public class ForumThreadEntryTOCItemsViewModel : ForumThreadEntriesViewModelBase
    {
        public int NumberOfReplies { get; set; }
        public IEnumerable<HierarchyNode<ForumThreadEntryTOCItemViewModel>> ForumThreadEntriesToc { get; set; }
    }

    public class ForumThreadEntryTOCItemViewModel : ForumThreadEntriesViewModelBase
    {
    }
    
    public class ForumThreadEntriesCreateViewModel : ForumThreadEntriesViewModelBase
    {
        public int ParentPostNumber { get; set; }
    }

    public class ForumThreadEntriesEditViewModel : ForumThreadEntriesViewModelBase
    {
    }
}

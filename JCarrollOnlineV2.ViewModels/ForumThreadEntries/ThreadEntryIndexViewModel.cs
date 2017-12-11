using JCarrollOnlineV2.ViewModels.Fora;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace JCarrollOnlineV2.ViewModels.ForumThreadEntries
{
    public class ThreadEntryIndexViewModel : ThreadEntriesViewModelBase
    {
        public List<ThreadEntryIndexItemViewModel> ForumThreadEntryIndex { get; set; }

        public ForaViewModel Forum { get; set; }

        [DataType(DataType.Text)]
        [StringLength(256)]
        [Display(Name = "Title")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string Title { get; set; }

        [DataType(DataType.MultilineText)]
        public string Content { get; set; }
    }
}

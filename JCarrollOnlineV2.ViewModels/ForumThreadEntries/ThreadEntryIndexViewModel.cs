using JCarrollOnlineV2.ViewModels.Fora;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace JCarrollOnlineV2.ViewModels.ForumThreadEntries
{
    public class ThreadEntryIndexViewModel : ThreadEntriesViewModelBase
    {
        private ICollection<ThreadEntryIndexItemViewModel> _threadEntryIndex = new List<ThreadEntryIndexItemViewModel>();

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public ICollection<ThreadEntryIndexItemViewModel> ThreadEntryIndex => _threadEntryIndex;

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

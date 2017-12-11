using PagedList;
using System.Collections.Generic;

namespace JCarrollOnlineV2.ViewModels.MicroPosts
{
    public class MicroPostFeedViewModel : MicroPostViewModelBase
    {
        public MicroPostFeedViewModel()
        {
            MicroPostFeedItems = new List<MicroPostFeedItemViewModel>();
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public ICollection<MicroPostFeedItemViewModel> MicroPostFeedItems { get; set; }
        public IPagedList<MicroPostFeedItemViewModel> OnePageOfMicroPosts { get; set; }
    }
}

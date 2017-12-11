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
        public List<MicroPostFeedItemViewModel> MicroPostFeedItems { get; set; }
        public IPagedList<MicroPostFeedItemViewModel> OnePageOfMicroPosts { get; set; }
    }
}

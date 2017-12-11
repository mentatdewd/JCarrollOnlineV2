using System.Collections.Generic;

namespace JCarrollOnlineV2.ViewModels.Blog
{
    public class BlogFeedViewModel : BlogFeedViewModelBase
    {
        public BlogFeedViewModel()
        {
            BlogFeedItemViewModels = new List<BlogFeedItemViewModel>();
        }
        public List<BlogFeedItemViewModel> BlogFeedItemViewModels { get; set; }
    }
}

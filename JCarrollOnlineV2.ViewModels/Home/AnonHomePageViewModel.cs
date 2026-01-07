using JCarrollOnlineV2.ViewModels.Blog;

namespace JCarrollOnlineV2.ViewModels.Home
{
    public class AnonHomepageViewModelBase : ViewModelBase
    {

    }

    public class AnonHomepageViewModel : AnonHomepageViewModelBase
    {
        public BlogFeedViewModel BlogFeed { get; set; }
    }
}

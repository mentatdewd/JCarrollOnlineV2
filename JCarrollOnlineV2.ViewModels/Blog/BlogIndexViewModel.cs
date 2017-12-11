namespace JCarrollOnlineV2.ViewModels.Blog
{
    public class BlogIndexViewModel : BlogFeedViewModelBase
    {
        public BlogIndexViewModel()
        {
            BlogFeedItems = new BlogFeedViewModel();
        }

        public BlogFeedViewModel BlogFeedItems { get; set; }
    }
}

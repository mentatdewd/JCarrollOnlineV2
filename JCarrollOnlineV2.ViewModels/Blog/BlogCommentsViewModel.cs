using System.Collections.Generic;

namespace JCarrollOnlineV2.ViewModels.Blog
{
    public class BlogCommentsViewModel : BlogFeedViewModelBase
    {
        public BlogCommentsViewModel()
        {
            BlogComments = new List<BlogCommentItemViewModel>();
        }
        public int BlogItemId { get; set; }
        public ICollection<BlogCommentItemViewModel> BlogComments { get; private set; }
    }
}

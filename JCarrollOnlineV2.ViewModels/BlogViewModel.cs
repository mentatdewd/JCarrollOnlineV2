using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace JCarrollOnlineV2.ViewModels
{
    public class BlogFeedViewModelBase : ViewModelBase
    {
    }

    public class BlogIndexViewModel : BlogFeedViewModelBase
    {
        public BlogIndexViewModel()
        {
            BlogFeedItems = new BlogFeedViewModel();
        }

        public BlogFeedViewModel BlogFeedItems { get; set; }
    }

    public class BlogFeedViewModel : BlogFeedViewModelBase
    {
        public BlogFeedViewModel()
        {
            BlogFeedItemVMs = new List<BlogFeedItemViewModel>();
        }
        public List<BlogFeedItemViewModel> BlogFeedItemVMs { get; set; }
    }

    public class BlogFeedItemViewModel : BlogFeedViewModelBase
    {
        public int Id { get; set; }
        public BlogFeedItemViewModel()
        {
            Author = new ApplicationUserViewModel();
        }
        public ApplicationUserViewModel Author { get; set; }
        [Display(Name="Title")]
        public string Title { get; set; }

        [Display(Name="Content")]
        [DataType(DataType.MultilineText)]

        [AllowHtml]
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}

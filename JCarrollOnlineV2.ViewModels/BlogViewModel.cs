using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public BlogFeedItemViewModel()
        {
            Author = new ApplicationUserViewModel();
            Comments = new BlogCommentsViewModel();
        }
        public int Id { get; set; }
        public ApplicationUserViewModel Author { get; set; }

        public string AuthorId { get; set; }

        [Display(Name="Title")]
        public string Title { get; set; }

        [Display(Name="Content")]
        [DataType(DataType.MultilineText)]

        [AllowHtml]
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public BlogCommentsViewModel Comments { get; set; }
    }

    public class BlogCommentsViewModel : BlogFeedViewModelBase
    {
        public BlogCommentsViewModel()
        {
            BlogComments = new List<BlogCommentItemViewModel>();
        }
        public int BlogItemId { get; set; }
        public List<BlogCommentItemViewModel> BlogComments { get; set; }
    }

    public class BlogCommentItemViewModel : BlogFeedViewModelBase
    {
        public int Id { get; set; }

        [RegularExpression("^[a-z0-9_-]{3,16}$", ErrorMessage=("User name must contain only lower case a-z, numbers, underscore,or hyphen and be at least 3 characters"))]
        [Required]
        [StringLength(16)]
        public string Author { get; set; }

        [Required]
        [StringLength(512)]
        public string Content { get; set; }

        public int BlogItemId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string TimeAgo { get; set; }

        public string ReturnUrl { get; set; }
    }
}

using JCarrollOnlineV2.ViewModels.Users;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace JCarrollOnlineV2.ViewModels.Blog
{
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

        [Display(Name = "Title")]
        public string Title { get; set; }

        [Display(Name = "Content")]
        [DataType(DataType.MultilineText)]
        [AllowHtml]
        public string Content { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public BlogCommentsViewModel Comments { get; set; }
    }
}

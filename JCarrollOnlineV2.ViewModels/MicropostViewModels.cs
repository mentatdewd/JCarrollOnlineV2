using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using JCarrollOnlineV2.Entities;
using PagedList;

namespace JCarrollOnlineV2.ViewModels
{
    public class MicroPostViewModelBase : ViewModelBase
    {
        public string UserId { get; set; }
        public string Email { get; set; }
    }

    public class MicroPostCreateViewModel : MicroPostViewModelBase
    {
        [DataType(DataType.MultilineText)]
        [StringLength(140)]
        public string Content { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } // :null => false

        [DataType(DataType.DateTime)]
        public DateTime UpdatedAt { get; set; } // :null => false
    }

    public class MicroPostFeedViewModel : MicroPostViewModelBase
    {
        public MicroPostFeedViewModel()
        {
            MicroPostFeedItems = new List<MicroPostFeedItemViewModel>();
        }
        public List<MicroPostFeedItemViewModel> MicroPostFeedItems { get; set; }
        public IPagedList<MicroPostFeedItemViewModel> OnePageOfMicroPosts { get; set; }
    }

    public class MicroPostFeedItemViewModel : MicroPostViewModelBase
    {
        public MicroPostFeedItemViewModel()
        {
            Author = new ApplicationUserViewModel();
        }
        public ApplicationUserViewModel Author { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public string TimeAgo { get; set; }
    }

    public class MicroPostDeleteViewModel : MicroPostViewModelBase
    {
        [DataType(DataType.MultilineText)]
        [StringLength(140)]
        public string Content { get; set; }

        [Required]
        public ApplicationUser Author { get; set; }

        [DataType(DataType.DateTime)]
        [Required]
        public DateTime CreatedAt { get; set; } // :null => false

        [DataType(DataType.DateTime)]
        [Required]
        public DateTime UpdatedAt { get; set; } // :null => false
    }

    public class MicroPostDetailsViewModel : MicroPostViewModelBase
    {
        [DataType(DataType.MultilineText)]
        [StringLength(140)]
        public string Content { get; set; }

        [Required]
        public ApplicationUser Author { get; set; }

        [DataType(DataType.DateTime)]
        [Required]
        public DateTime CreatedAt { get; set; } // :null => false

        [DataType(DataType.DateTime)]
        [Required]
        public DateTime UpdatedAt { get; set; } // :null => false
    }

    public class MicroPostEditViewModel : MicroPostViewModelBase
    {
        [DataType(DataType.MultilineText)]
        [StringLength(140)]
        public string Content { get; set; }

        [Required]
        public ApplicationUser Author { get; set; }

        [DataType(DataType.DateTime)]
        [Required]
        public DateTime CreatedAt { get; set; } // :null => false

        [DataType(DataType.DateTime)]
        [Required]
        public DateTime UpdatedAt { get; set; } // :null => false
    }

    public class MicroPostIndexViewModel : MicroPostViewModelBase
    {
        [DataType(DataType.MultilineText)]
        [StringLength(140)]
        public string Content { get; set; }

        [Required]
        public ApplicationUser Author { get; set; }

        [DataType(DataType.DateTime)]
        [Required]
        public DateTime CreatedAt { get; set; } // :null => false

        [DataType(DataType.DateTime)]
        [Required]
        public DateTime UpdatedAt { get; set; } // :null => false
    }
}

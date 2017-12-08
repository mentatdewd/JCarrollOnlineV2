using System;
using System.ComponentModel.DataAnnotations;
namespace JCarrollOnlineV2.ViewModels
{
    public class ForumModeratorsViewModelBase : ViewModelBase
    {
    }

    public class ForumModeratorsViewModel : ForumModeratorsViewModelBase
    {

    }

    public class ForumModeratorsCreateViewModel : ForumModeratorsViewModelBase
    {
        public int ForumId { get; set; }
        public DateTime CreatedAt { get; set; }//  :null => false
        public DateTime UpdatedAt { get; set; }//   :null => false
    }

    public class ForumModeratorsDeleteViewModel : ForumModeratorsViewModelBase
    {
        public int ForumId { get; set; }
        public DateTime CreatedAt { get; set; }//  :null => false
        public DateTime UpdatedAt { get; set; }//   :null => false
    }

    public class ForumModeratorsDetailsViewModel : ForumModeratorsViewModelBase
    {
        public int ForumId { get; set; }
        public DateTime CreatedAt { get; set; }//  :null => false
        public DateTime UpdatedAt { get; set; }//   :null => false

    }

    public class ForumModeratorsEditViewModel : ForumModeratorsViewModelBase
    {
        public int ForumId { get; set; }
        public DateTime CreatedAt { get; set; }//  :null => false
        public DateTime UpdatedAt { get; set; }//   :null => false
    }

    public class ForumModeratorsIndexViewModel : ForumModeratorsViewModelBase
    {
        [Display(Name="Forum Id")]
        public int ForumId { get; set; }

        [Display(Name="Created At")]
        public DateTime CreatedAt { get; set; }//  :null => false

        [Display(Name="Updated At")]
        public DateTime UpdatedAt { get; set; }//   :null => false
    }
}

using System;
using System.ComponentModel.DataAnnotations;

namespace JCarrollOnlineV2.ViewModels.ForumModerators
{
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

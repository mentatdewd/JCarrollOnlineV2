using System;

namespace JCarrollOnlineV2.ViewModels.ForumModerators
{
    public class ForumModeratorsCreateViewModel : ForumModeratorsViewModelBase
    {
        public int ForumId { get; set; }
        public DateTime CreatedAt { get; set; }//  :null => false
        public DateTime UpdatedAt { get; set; }//   :null => false
    }
}

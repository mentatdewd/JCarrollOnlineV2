using System;

namespace JCarrollOnlineV2.ViewModels.ForumModerators
{
    public class ForumModeratorsDeleteViewModel : ForumModeratorsViewModelBase
    {
        public int ForumId { get; set; }
        public DateTime CreatedAt { get; set; }//  :null => false
        public DateTime UpdatedAt { get; set; }//   :null => false
    }
}

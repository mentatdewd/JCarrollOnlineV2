using JCarrollOnlineV2.ViewModels.MicroPosts;
using NLog;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace JCarrollOnlineV2.ViewModels.Users
{
    public class UserItemViewModel : UserViewModelBase
    {
        public UserItemViewModel() { }
        public UserItemViewModel(ILogger logger)
        {
            Logger = logger;
        }

        public string UserId { get; set; }

        [Display(Name = "MicroPost Email Notifications")]
        public bool MicroPostEmailNotifications { get; set; }

        [Display(Name = "MicroPost SMS Notifications")]
        public bool MicroPostSmsNotifications { get; set; }

        public int? MicroPostsAuthored { get; set; }
        public ICollection<MicroPostFeedItemViewModel> MicroPosts { get; set; }
        public ILogger Logger { get; set; }
    }
}

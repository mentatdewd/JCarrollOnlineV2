using Microsoft.AspNet.Identity;
using System.Collections.Generic;

namespace JCarrollOnlineV2.ViewModels.Manage
{
    public class ManageIndexViewModel : ViewModelBase
    {
        public bool HasPassword { get; set; }
        public IList<UserLoginInfo> Logins { get; set; }
        public string PhoneNumber { get; set; }
        public bool TwoFactor { get; set; }
        public bool BrowserRemembered { get; set; }
        public string StatusMessage { get; set; }
    }
}

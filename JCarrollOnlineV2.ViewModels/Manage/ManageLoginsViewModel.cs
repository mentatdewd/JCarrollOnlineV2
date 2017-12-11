using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System.Collections.Generic;

namespace JCarrollOnlineV2.ViewModels.Manage
{
    public class ManageLoginsViewModel : ViewModelBase
    {
        public IList<UserLoginInfo> CurrentLogins { get; set; }
        public IList<AuthenticationDescription> OtherLogins { get; set; }
        public string StatusMessage { get; set; }
        public bool ShowRemoveButton { get; set; }
    }
}

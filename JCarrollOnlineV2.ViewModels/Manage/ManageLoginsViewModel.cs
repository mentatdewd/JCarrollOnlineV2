using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System.Collections.Generic;

namespace JCarrollOnlineV2.ViewModels.Manage
{
    public class ManageLoginsViewModel : ViewModelBase
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public ICollection<UserLoginInfo> CurrentLogins { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public ICollection<AuthenticationDescription> OtherLogins { get; set; }
        public string StatusMessage { get; set; }
        public bool ShowRemoveButton { get; set; }
    }
}

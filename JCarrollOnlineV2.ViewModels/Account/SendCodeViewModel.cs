using System.Collections.Generic;

namespace JCarrollOnlineV2.ViewModels.Account
{
    public class SendCodeViewModel : ViewModelBase
    {
        public string SelectedProvider { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings")]
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }
}

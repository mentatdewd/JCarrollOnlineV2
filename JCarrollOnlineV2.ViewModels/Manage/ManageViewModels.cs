using System.Collections.Generic;

namespace JCarrollOnlineV2.ViewModels.Manage
{
    public class ManageConfigureTwoFactorViewModel : ViewModelBase
    {
        public string SelectedProvider { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
    }
}
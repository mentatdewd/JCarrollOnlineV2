using System.Collections.Generic;

namespace JCarrollOnlineV2.ViewModels.Account
{
    public class SendCodeViewModel : ViewModelBase
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }
}

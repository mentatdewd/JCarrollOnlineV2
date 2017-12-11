using System.ComponentModel.DataAnnotations;

namespace JCarrollOnlineV2.ViewModels.Account
{
    public class ExternalLoginConfirmationViewModel : ViewModelBase
    {
        [Required]
        public string SiteUserName { get; set; }
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
        public string ReturnUrl { get; set; }
        public string LoginProvider { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace JCarrollOnlineV2.ViewModels.Account
{
    public class ForgotPasswordViewModel : ViewModelBase
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

}

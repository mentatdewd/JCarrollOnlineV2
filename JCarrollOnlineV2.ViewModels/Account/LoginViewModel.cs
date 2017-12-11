using System.ComponentModel.DataAnnotations;

namespace JCarrollOnlineV2.ViewModels.Account
{
    public class LoginViewModel : ViewModelBase
    {
        [Required]
        [Display(Name = "Name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }
        public string LoginProvider { get; set; }
    }
}

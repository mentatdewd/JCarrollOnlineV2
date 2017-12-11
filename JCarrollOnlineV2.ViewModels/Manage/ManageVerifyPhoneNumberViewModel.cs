using System.ComponentModel.DataAnnotations;

namespace JCarrollOnlineV2.ViewModels.Manage
{
    public class ManageVerifyPhoneNumberViewModel : ViewModelBase
    {
        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
        public string Status { get; set; }
    }
}

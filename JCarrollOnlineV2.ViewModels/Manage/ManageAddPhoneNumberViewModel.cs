using System.ComponentModel.DataAnnotations;

namespace JCarrollOnlineV2.ViewModels.Manage
{
    public class ManageAddPhoneNumberViewModel : ViewModelBase
    {
        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string Number { get; set; }
    }
}

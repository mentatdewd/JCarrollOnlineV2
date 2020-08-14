using System.ComponentModel.DataAnnotations;

namespace JCarrollOnlineV2.ViewModels.Users
{
    public class ApplicationUserViewModel : ApplicationUserViewModelBase
    {
        public string Id { get; set; }

        public string Email { get; set; }

        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Display(Name="Enable microPost Email notifications")]
        public bool MicroPostEmailNotifications { get; set; }

        [Display(Name="Enable microPost SMS notifications")]
        public bool MicroPostSMSNotifications { get; set; }
    }
}

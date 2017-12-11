using System.ComponentModel.DataAnnotations;

namespace JCarrollOnlineV2.ViewModels.Account
{
    public class DeleteUserViewModel : ViewModelBase
    {
        public string Id { get; set; }

        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }
    }

}

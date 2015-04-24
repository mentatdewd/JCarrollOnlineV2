using System;
using System.ComponentModel.DataAnnotations;

namespace JCarrollOnlineV2.ViewModels
{
    public class ApplicationUserViewModelBase : ViewModelBase
    {
    }

    public class ApplicationUserViewModel : ApplicationUserViewModelBase
    {
        public string Id { get; set; }

        public string Email { get; set; }

        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; }
    }
}

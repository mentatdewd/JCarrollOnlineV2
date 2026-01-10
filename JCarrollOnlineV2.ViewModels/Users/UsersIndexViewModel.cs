using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace JCarrollOnlineV2.ViewModels.Users
{
    public class UsersIndexViewModel : UserViewModelBase
    {
        public UsersIndexViewModel()
        {
            Users = new List<UserItemViewModel>();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public ICollection<UserItemViewModel> Users { get; set; }

        // Mass Email Properties
        [Required(ErrorMessage = "Subject is required")]
        [StringLength(200, ErrorMessage = "Subject cannot exceed 200 characters")]
        [Display(Name = "Email Subject")]
        public string EmailSubject { get; set; }

        [Required(ErrorMessage = "Message body is required")]
        [Display(Name = "Email Message")]
        public string EmailBody { get; set; }

        [Display(Name = "Enable HTML formatting")]
        public bool IsHtml { get; set; }
    }
}

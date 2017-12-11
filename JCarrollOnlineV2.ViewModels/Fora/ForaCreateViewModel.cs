using System;
using System.ComponentModel.DataAnnotations;

namespace JCarrollOnlineV2.ViewModels.Fora
{
    public class ForaCreateViewModel : ForaViewModelBase
    {
        [Required]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

}

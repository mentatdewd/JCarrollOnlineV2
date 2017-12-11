using System;
using System.ComponentModel.DataAnnotations;

namespace JCarrollOnlineV2.ViewModels.Fora
{
    public class ForumDeleteViewModel : ForaViewModelBase
    {
        [Required]
        [StringLength(255)]
        public string Title { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [DataType(DataType.DateTime)]
        [Required]
        public DateTime CreatedAt { get; set; } // :null => false

        [DataType(DataType.DateTime)]
        [Required]
        public DateTime UpdatedAt { get; set; } //:null => false

    }
}

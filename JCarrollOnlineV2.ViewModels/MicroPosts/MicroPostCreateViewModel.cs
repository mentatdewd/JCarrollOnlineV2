using System;
using System.ComponentModel.DataAnnotations;

namespace JCarrollOnlineV2.ViewModels.MicroPosts
{
    public class MicroPostCreateViewModel : MicroPostViewModelBase
    {
        [DataType(DataType.MultilineText)]
        [StringLength(140)]
        public string Content { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } // :null => false

        [DataType(DataType.DateTime)]
        public DateTime UpdatedAt { get; set; } // :null => false
    }
}

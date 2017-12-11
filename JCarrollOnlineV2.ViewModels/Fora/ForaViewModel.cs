using JCarrollOnlineV2.ViewModels.ForumModerators;
using JCarrollOnlineV2.ViewModels.ForumThreadEntries;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace JCarrollOnlineV2.ViewModels.Fora
{
    public class ForaViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Title { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } // :null => false

        [DataType(DataType.DateTime)]
        public DateTime UpdatedAt { get; set; } //:null => false

        public List<ThreadEntryViewModel> ForumThreadEntries { get; set; }
        public List<ForumModeratorsViewModel> ForumModerators { get; set; }
    }
}

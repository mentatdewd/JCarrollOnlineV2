using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace JCarrollOnlineV2.ViewModels
{
    public class ForaViewModelBase : ViewModelBase
    {
        public int Id { get; set; }
    }

    public class ForaIndexViewModel : ForaViewModelBase
    {
        public List<ForaIndexItemViewModel> ForaIndexItems { get; set; }
    }

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

        public List<ForumThreadEntryViewModel> ForumThreadEntries { get; set; }
        public List<ForumModeratorsViewModel> ForumModerators { get; set; }
    }


    public class ForaIndexItemViewModel : ForaViewModelBase
    {
        [Display(Name = "Title")]
        public string Title { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Thread Count")]
        public int ThreadCount { get; set; }

        [Display(Name = "Last Thread")]
        public LastThreadViewModel LastThread { get; set; }
    }

    public class ForaDetailsViewModel : ForaViewModelBase
    {
        [Required]
        [StringLength(255)]
        public string Title { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } // :null => false

        [DataType(DataType.DateTime)]
        public DateTime UpdatedAt { get; set; } //:null => false

        public List<ForumThreadEntryViewModel> ForumThreadEntries { get; set; }
        public List<ForumModeratorsViewModel> ForumModerators { get; set; }
    }

    public class LastThreadViewModel : ForaViewModelBase
    {
        public DateTime UpdatedAt { get; set; }
        public string Title { get; set; }
        public int PostRoot { get; set; }
        public int PostNumber { get; set; }
        public ApplicationUserViewModel Author { get; set; }
        public ForaViewModel Forum { get; set; }
    }

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
    public class ForumEditViewModel : ForaViewModelBase
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


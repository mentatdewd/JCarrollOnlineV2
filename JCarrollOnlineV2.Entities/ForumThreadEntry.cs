using System;
using System.ComponentModel.DataAnnotations;

namespace JCarrollOnlineV2.Entities
{
    public class ForumThreadEntry
    {
        [Key]
        public int Id { get; set; }

        [StringLength(255)]
        public string Title { get; set; }

        [DataType(DataType.MultilineText)]
        public string Content { get; set; }

        public bool Locked { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } //           :null => false

        [Required]
        public DateTime UpdatedAt { get; set; } //          :null => false

        [Required]
        public int PostNumber { get; set; }

        public int? ParentId { get; set; }

        public int? RootId { get; set; }

        [Required]
        public virtual ApplicationUser Author { get; set; }

        [Required]
        public virtual Forum Forum { get; set; }
    }
}

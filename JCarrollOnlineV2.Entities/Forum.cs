using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JCarrollOnlineV2.Entities
{
    [Table("Fora")]
    public class Forum
    {
        [Required]
        public int Id { get; set; }

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

        public virtual List<ThreadEntry> ForumThreadEntries { get; set; }
        public virtual List<ForumModerator> ForumModerators { get; set; }
    }
}

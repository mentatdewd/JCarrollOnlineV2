using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCarrollOnlineV2.Entities
{
    public class ForumThreadEntry
    {
        [Key]
        [Column("Id")]
        public int ForumThreadEntryId { get; set; }

        [DataType(DataType.Text)]
        [StringLength(256)]
        public string Title { get; set; }

        [DataType(DataType.MultilineText)]
        public string Content { get; set; }

        public string AuthorId { get; set; }

        public bool Locked { get; set; }

        public int ForumId { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } //           :null => false

        [DataType(DataType.DateTime)]
        public DateTime UpdatedAt { get; set; } //          :null => false

        public int PostNumber { get; set; }

        public int? ParentId { get; set; }
        //public virtual ForumThreadEntry Parent { get; set; }
        //public virtual ICollection<ForumThreadEntry> Children { get; set; }
    }
}

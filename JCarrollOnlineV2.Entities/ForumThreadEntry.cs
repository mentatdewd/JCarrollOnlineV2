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
        public int Id { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public bool Locked { get; set; }

        public DateTime CreatedAt { get; set; } //           :null => false

        public DateTime UpdatedAt { get; set; } //          :null => false

        public int PostNumber { get; set; }

        public int? ParentId { get; set; }

        public int? RootId { get; set; }

        public virtual ApplicationUser Author { get; set; }

        public virtual Forum Forum { get; set; }
    }
}

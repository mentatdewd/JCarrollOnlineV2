using JCarrollOnlineV2.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCarrollOnlineV2.Entities
{
    public class Micropost
    {
        public int Id { get; set; }

        [DataType(DataType.MultilineText)]
        [StringLength(140)]
        public string Content { get; set; }

        public string UserId { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } // :null => false

        [DataType(DataType.DateTime)]
        public DateTime UpdatedAt { get; set; } // :null => false

        public virtual ICollection<Relationship> FollowerIds { get; set; }
        public virtual ICollection<Relationship> FollowedIds { get; set; }
    }
}

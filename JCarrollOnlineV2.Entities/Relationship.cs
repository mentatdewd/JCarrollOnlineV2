using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using JCarrollOnlineV2.ViewModels;

namespace JCarrollOnlineV2.Entities
{
    public class Relationship
    {
        [Key, Column(Order=0)]
        public string FollowerId { get; set; }

        [Key, Column(Order=1)]
        public string FollowedId { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; }//  :null => false

        [DataType(DataType.DateTime)]
        public DateTime UpdatedAt { get; set; }//  :null => false

        [Required]
        public virtual ApplicationUser ApplicationUser { get; set; }

        [Required]
        public virtual Micropost Micropost { get; set; }
    }
}

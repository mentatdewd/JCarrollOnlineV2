using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JCarrollOnlineV2.Entities
{
    public class Relationship
    {
        public int Id { get; set; }

        [Index("IX_FirstAndSecond", 1, IsUnique=true)]
        public string FollowerId { get; set; }

        [Index("IX_FirstAndSecond", 2, IsUnique=true)]
        public string FollowedId { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; }//  :null => false

        [DataType(DataType.DateTime)]
        public DateTime UpdatedAt { get; set; }//  :null => false

        [Required]
        public virtual ICollection<ApplicationUser> ApplicationUser { get; set; }

        [Required]
        public virtual Micropost Micropost { get; set; }
    }
}

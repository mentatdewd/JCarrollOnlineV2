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

        [Index("FollowerIndex", IsUnique=false)]
        public int FollowerId { get; set; }

        [Index("FollowedIndex", IsUnique=false)]
        [Index("FollowerAndFollowedIndex", 2, IsUnique=true)]
        public int FollowedId { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; }//  :null => false

        [DataType(DataType.DateTime)]
        public DateTime UpdatedAt { get; set; }//  :null => false
    }
}

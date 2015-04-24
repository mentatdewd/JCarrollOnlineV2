using System;

namespace JCarrollOnlineV2.Entities
{
    public class ForumModerator
    {
        public int Id { get; set; }
        public int ForumId { get; set; }
        public DateTime CreatedAt { get; set; }//  :null => false
        public DateTime UpdatedAt { get; set; }//   :null => false
    }
}

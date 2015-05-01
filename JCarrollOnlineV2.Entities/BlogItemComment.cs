using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCarrollOnlineV2.Entities
{
    public class BlogItemComment
    {
        [Key]
        public int Id { get; set; }
        public string Author { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Content { get; set; }

        public virtual BlogItem BlogItem { get; set; }
    }
}

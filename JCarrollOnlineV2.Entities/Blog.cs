using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace JCarrollOnlineV2.Entities
{
    public class BlogItem
    {
        [Key]
        public int Id { get; set; }

        [AllowHtml]
        [Required]
        public string Content { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
        [Required]
        public DateTime UpdatedAt { get; set; }

        [Required]
        public virtual ApplicationUser Author { get; set; }
        public virtual List<BlogItemComment> BlogItemComments { get; set; }
    }
}

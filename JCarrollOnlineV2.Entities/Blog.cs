using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace JCarrollOnlineV2.Entities
{
    public class Blog
    {
        [Key]
        public int Id { get; set; }

       //public ApplicationUser Author { get; set; }
        public string Title { get; set; }

        [AllowHtml]
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public virtual ApplicationUser Author { get; set; }
    }
}

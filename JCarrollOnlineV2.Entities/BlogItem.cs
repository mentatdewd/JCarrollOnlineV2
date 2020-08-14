using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
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

        public string Title { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }
        [Required]
        public DateTime UpdatedAt { get; set; }

        [Required]
        public virtual ApplicationUser Author { get; set; }
#pragma warning disable CA2227 // Collection properties should be read only injector needs access to this
        public virtual Collection<BlogItemComment> BlogItemComments { get; set; }
#pragma warning restore CA2227 // Collection properties should be read only
    }
}

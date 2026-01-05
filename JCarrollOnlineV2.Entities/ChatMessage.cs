using System;
using System.ComponentModel.DataAnnotations;

namespace JCarrollOnlineV2.Entities
{
    public class ChatMessage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(500)]
        public string Message { get; set; }

        [Required]
        public string AuthorId { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public virtual ApplicationUser Author { get; set; }
    }
}
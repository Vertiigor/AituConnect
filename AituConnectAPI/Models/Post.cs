using AituConnectAPI.Models.Abstractions;
using System.ComponentModel.DataAnnotations;

namespace AituConnectAPI.Models
{
    public class Post : IHaveAuthor
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public DateTime CreationDate { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public string AuthorId { get; set; }
    }
}

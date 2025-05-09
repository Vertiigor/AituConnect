using System.ComponentModel.DataAnnotations;

namespace MessageListenerService.Models
{
    public class Post : IHaveAuthor
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public string University { get; set; }

        [Required]
        public List<Subject> Subjects { get; set; }

        [Required]
        public Status Status { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public enum Status
    {
        Draft,
        Published,
        Archived
    }
}

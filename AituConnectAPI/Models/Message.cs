using System.ComponentModel.DataAnnotations;

namespace AituConnectAPI.Models
{
    public class Message
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public string ChatId { get; set; }

        [Required]
        public string MessageId { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public DateTime SentTime { get; set; }
    }
}

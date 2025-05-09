using System.ComponentModel.DataAnnotations;

namespace MessageListenerService.Models
{
    public class Subject
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}

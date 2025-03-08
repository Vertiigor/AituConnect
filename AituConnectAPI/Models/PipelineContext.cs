using System.ComponentModel.DataAnnotations;

namespace AituConnectAPI.Models
{
    public class PipelineContext
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public string ChatId { get; set; }

        [Required]
        public string Type { get; set; }

        [Required]
        public string CurrentStep { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public bool IsCompleted { get; set; } = false;

        [Required]
        public DateTime StartedDate { get; set; }

        [Required]
        public DateTime FinishedDate { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace AituConnectAPI.Models.Abstractions
{
    public class PipelineContext
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public string ChatId { get; set; }

        [Required]
        public string CurrentStep { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public bool IsCompleted { get; set; } = false;
    }
}

using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace MessageListenerService.Models
{
    public class User : IdentityUser
    {
        [Required]
        public string ChatId { get; set; }

        [Required]
        public string University { get; set; }

        [Required]
        public string Major { get; set; }

        [Required]
        public DateTime JoinedDate { get; set; }

        [Required]
        public Roles Role { get; set; }
    }

    public enum Roles
    {
        Admin,
        User
    }
}

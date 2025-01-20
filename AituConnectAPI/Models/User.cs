using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AituConnectAPI.Models
{
    public class User : IdentityUser
    {   
        [Required]
        public string ChatId { get; set; }

        [Required]
        public DateTime JoinedDate { get; set; }

        public Roles Role { get; set; }
    }

    public enum Roles
    {
        ADMIN,
        USER
    }
}

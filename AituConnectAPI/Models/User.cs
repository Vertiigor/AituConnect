using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AituConnectAPI.Models
{
    public class User : IdentityUser
    {   
        [Required]
        public string ChatId { get; set; }

    }
}

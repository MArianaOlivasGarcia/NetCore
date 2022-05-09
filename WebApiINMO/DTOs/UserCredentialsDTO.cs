using System.ComponentModel.DataAnnotations;

namespace WebApiINMO.DTOs
{
    public class UserCredentialsDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }

    }
}

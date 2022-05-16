using System.ComponentModel.DataAnnotations;

namespace WebApiINMO.DTOs
{
    public class AddRoleDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required] 
        // TODO: Validar que sea ADMIN o USER en mayuscula
        public string Role { get; set; }
    }
}

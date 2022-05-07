using System.ComponentModel.DataAnnotations;

namespace WebApiINMO.DTOs
{
    public class AdviserCreateDTO
    {

        [Required(ErrorMessage = "El {0} es requerido.")]
        [StringLength(maximumLength: 120, ErrorMessage = "El {0} no debe tener más de {1} carácteres.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "El {0} es requerido.")]
        [StringLength(maximumLength: 120, ErrorMessage = "El {0} no debe tener más de {1} carácteres.")]
        public string LastName { get; set; }

    }
}

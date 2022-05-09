using System.ComponentModel.DataAnnotations;

namespace WebApiINMO.DTOs
{
    public class PropertyPatchDTO
    {

        [Required(ErrorMessage = "El {0} es requerido.")]
        [StringLength(maximumLength: 400, ErrorMessage = "El {0} no debe tener más de {1} carácteres.")]
        public string Name { get; set; }
        public string Description { get; set; }

    }
}

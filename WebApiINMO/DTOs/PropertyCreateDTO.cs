using System.ComponentModel.DataAnnotations;

namespace WebApiINMO.DTOs
{
    public class PropertyCreateDTO
    {

        [Required(ErrorMessage = "El {0} es requerido.")]
        [StringLength(maximumLength: 400, ErrorMessage = "El {0} no debe tener más de {1} carácteres.")]
        public string Name { get; set; }
        public int AdviserId { get; set; }
        public List<int> AmenitiesIds { get; set; }


    }
}

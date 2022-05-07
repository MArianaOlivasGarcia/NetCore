using System.ComponentModel.DataAnnotations;

namespace WebApiINMO.Entities
{
    public class Amenity
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El {0} es requerido.")]
        [StringLength(maximumLength: 80, ErrorMessage = "El {0} no debe tener más de {1} carácteres.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "El {0} es requerido.")]
        [StringLength(maximumLength: 80, ErrorMessage = "El {0} no debe tener más de {1} carácteres.")]
        public string Icon { get; set; }


        // Propiedades de navegación
        public List<PropertyAmenity> PropertyAmenity { get; set; }

    }
}

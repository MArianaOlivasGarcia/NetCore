using System.ComponentModel.DataAnnotations;

namespace WebApiINMO.Entities
{
    public class Property
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El {0} es requerido.")]
        [StringLength(maximumLength: 400, ErrorMessage = "El {0} no debe tener más de {1} carácteres.")]
        public string Name { get; set; }
        public string? Description { get; set; }
        public int AdviserId { get; set; }

        //Propiedades de navegacín
        public Adviser Adviser { get; set; }
        public List<PropertyAmenity> PropertyAmenity { get; set; }

    }
}

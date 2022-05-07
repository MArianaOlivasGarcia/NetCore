using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApiINMO.Validators;

namespace WebApiINMO.Entities
{
    public class Adviser
    {

        public int Id { get; set; }

        [Required(ErrorMessage = "El {0} es requerido.")]
        [StringLength( maximumLength:120, ErrorMessage = "El {0} no debe tener más de {1} carácteres.")]
        //[FirstCapitalLetter]
        public string Name { get; set; }

        [Required(ErrorMessage = "El {0} es requerido.")]
        [StringLength(maximumLength: 120, ErrorMessage = "El {0} no debe tener más de {1} carácteres.")]
        public string LastName { get; set; }

        // [Range(10, 120, ErrorMessage = "El campo {0} debe de estar en un rango de 10 y 120")]
        //  [NotMapped]
        // public int Edad { get; set; }

        // [CreditCard]
        // [NotMapped]
        // public string CreditCard { get; set; }


        // [Url]
        // [NotMapped]
        // public string Url { get; set; }

        // Propiedad de navegación
        // Permite cargar las propiedades de un asesor
        public List<Property> Properties { get; set; }


     


    }
}

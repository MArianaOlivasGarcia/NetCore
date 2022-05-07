namespace WebApiINMO.Entities
{
    public class PropertyAmenity
    {

        public int PropertyId { get; set; }
        public int AmenityId { get; set; }

        // Propiedades de navegación
        public Property Property { get; set; }
        public Amenity Amenity { get; set; }


    }
}

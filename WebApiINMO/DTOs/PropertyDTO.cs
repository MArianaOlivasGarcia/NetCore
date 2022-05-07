using WebApiINMO.Entities;

namespace WebApiINMO.DTOs
{
    public class PropertyDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public AdviserDTO Adviser { get; set; }

        public List<AmenityDTO> Amenities { get; set; }
    }
}

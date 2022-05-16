using Microsoft.AspNetCore.Identity;
using WebApiINMO.Entities;

namespace WebApiINMO.DTOs
{
    public class PropertyDTO: Resource
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string? Description { get; set; }


        public AdviserDTO Adviser { get; set; }
        public UserDTO User { get; set; }
        public List<AmenityDTO> Amenities { get; set; }

    }
}

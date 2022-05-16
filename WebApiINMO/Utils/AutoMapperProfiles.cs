using AutoMapper;
using Microsoft.AspNetCore.Identity;
using WebApiINMO.DTOs;
using WebApiINMO.Entities;

namespace WebApiINMO.Utils
{
    public class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles()
        {
            // Desde donde voy a mapear y hasta donde
            CreateMap<AdviserCreateDTO, Adviser>();
            CreateMap<Adviser, AdviserDTO>();


            // Añadir configuración para el mapeo de los id de las amenidades
            CreateMap<PropertyCreateDTO, Property>()
                .ForMember(property => property.PropertyAmenity, opt => opt.MapFrom( MapPropertiesAmenities ));
            CreateMap<Property, PropertyDTO>()
                .ForMember(property => property.Amenities, opt => opt.MapFrom( MapPropertyDTOAmenities));
            CreateMap<PropertyPatchDTO, Property>().ReverseMap();

            CreateMap<AmenityCreateDTO, Amenity>();
            CreateMap<Amenity, AmenityDTO>();


            CreateMap<IdentityUser, UserDTO>();




        }


        private List<PropertyAmenity> MapPropertiesAmenities(PropertyCreateDTO propertyDTO, Property property)
        {
            var result = new List<PropertyAmenity>();

            if ( propertyDTO.AmenitiesIds == null )
            {
                return result;
            }


            foreach ( var amenityId in propertyDTO.AmenitiesIds )
            {
                result.Add(new PropertyAmenity() { AmenityId = amenityId });
            }

            return result;
        }


        private List<AmenityDTO> MapPropertyDTOAmenities(Property property, PropertyDTO propertyDto)
        {
            var result = new List<AmenityDTO>();

            if ( property.PropertyAmenity == null ) { return result; }

            foreach ( var propertyAmenity in property.PropertyAmenity)
            {
                result.Add(new AmenityDTO()
                {
                    Id = propertyAmenity.AmenityId,
                    Name = propertyAmenity.Amenity.Name,
                    Icon = propertyAmenity.Amenity.Icon,
                });
            }

            return result;

        }

    }
}

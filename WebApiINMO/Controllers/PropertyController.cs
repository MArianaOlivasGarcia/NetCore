using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiINMO.DTOs;
using WebApiINMO.Entities;

namespace WebApiINMO.Controllers
{
    [ApiController]
    [Route("api/properties")]
    public class PropertyController: ControllerBase
    {


        private readonly ApplicationDbContext Context;
        private readonly IMapper Mapper;


        public PropertyController( ApplicationDbContext context, IMapper mapper )
        {
            Context = context;
            Mapper = mapper;
        }





        [HttpGet]
        public async Task<ActionResult<List<PropertyDTO>>> GetAll()
        {
            var properties = await Context.Properties
                    .Include(x => x.Adviser)
                    .Include(x => x.PropertyAmenity)
                    .ThenInclude(pa => pa.Amenity)
                    .ToListAsync();

            return Mapper.Map<List<PropertyDTO>>(properties);
        }


        [HttpGet("{id:int}", Name = "GetPropertyById")]
        public async Task<ActionResult<PropertyDTO>> GetById(int id)
        {
            var property = await Context.Properties
                .Include(x => x.Adviser)
                .Include(x => x.PropertyAmenity )
                .ThenInclude( pa => pa.Amenity )
                .FirstOrDefaultAsync(x => x.Id == id);
        
            if ( property == null )
            {
                return NotFound();
            }

            return Mapper.Map<PropertyDTO>(property);
        }




        [HttpGet("{name}")]
        public async Task<ActionResult<List<PropertyDTO>>> GetAllByName(string name)
        {
            var properties = await Context.Properties
                    .Include(x => x.Adviser)
                    .Include(x => x.PropertyAmenity)
                    .ThenInclude(pa => pa.Amenity)
                    .Where(x => x.Name.Contains(name))
                    .ToListAsync();
            return Mapper.Map<List<PropertyDTO>>(properties);
        }




        [HttpPost]
        public async Task<ActionResult> Create(PropertyCreateDTO propertyCreateDTO)
        {

            //if (propertyDTO.AmenitiesIds == null )
            //{
            //return BadRequest("Las amenidades son requeridas");
            //}

            if (propertyCreateDTO.AmenitiesIds != null) { 
            

                var amenitiesIds = await Context.Amenities
                    .Where(x => propertyCreateDTO.AmenitiesIds.Contains(x.Id))
                    .Select(x => x.Id)
                    .ToListAsync();

                // Si alguno de los id de las amenidades no esta,
                // significa que mando uno que no es válid

                if (propertyCreateDTO.AmenitiesIds.Count != amenitiesIds.Count)
                {
                    return BadRequest("No existe alguno de las amenidades.");
                }

            }

            var property = Mapper.Map<Property>(propertyCreateDTO);

            Context.Add(property);

            await Context.SaveChangesAsync();

            var propertyDTO = Mapper.Map<PropertyDTO>(property);


            //return Ok();


            return CreatedAtRoute("GetPropertyById", new { id = property.Id }, propertyDTO);

        }




    }
}

using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
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
        [Authorize( AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme )]
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

            var newProperty = await Context.Properties.AsNoTracking()
                    .Include(p => p.Adviser)
                    .Include(p => p.PropertyAmenity)
                    .ThenInclude(pa => pa.Amenity)
                    .FirstAsync(p => p.Id == property.Id);

            var propertyDTO = Mapper.Map<PropertyDTO>(newProperty);


            //return Ok();


            return CreatedAtRoute("GetPropertyById", new { id = property.Id }, propertyDTO);

        }


        [HttpPut("{id:int}")]
        public async Task<ActionResult> Update(int id, PropertyCreateDTO propertyCreateDTO)
        {
            var adviserdb = await Context.Properties
                    .Include(x => x.PropertyAmenity)
                    .FirstOrDefaultAsync(x => x.Id == id);

            if (adviserdb == null)
            {
                return NotFound();
            }

            // "Llevar las propiedades de propertyCreateDTO hacia adviserdb"
            // y asigno el resultado en la misma instancia adviserdb
            adviserdb = Mapper.Map(propertyCreateDTO, adviserdb);

            await Context.SaveChangesAsync();

            return NoContent();
        }



        [HttpPatch("{id:int}")]
        public async Task<ActionResult> Patch(int id, JsonPatchDocument<PropertyPatchDTO> patchDocument)
        {

            if ( patchDocument == null)
            {
                return BadRequest();
            }

            var propertydb = await Context.Properties.FirstOrDefaultAsync(x => x.Id == id);

            if ( propertydb == null )
            {
                return NotFound();
            }

            var propertyDTO = Mapper.Map<PropertyPatchDTO>(propertydb);

            // Llenando el pachDocument con la información del libro de la base de datos
            patchDocument.ApplyTo(propertyDTO, ModelState);

            // Verificar las reglas de validación 
            var isValid = TryValidateModel(propertyDTO);

            if ( !isValid )
            {
                return BadRequest(ModelState);
            }

            Mapper.Map(propertyDTO, propertydb);

            await Context.SaveChangesAsync();

            return NoContent();

        }



    }
}

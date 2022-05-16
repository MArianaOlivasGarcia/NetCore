using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiINMO.DTOs;
using WebApiINMO.Entities;
using WebApiINMO.Utils;

namespace WebApiINMO.Controllers.v1
{
    [ApiController]
    [Route("api/v1/properties")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    //[ApiConventionType(typeof(DefaultApiConventions))]
    public class PropertyController: ControllerBase
    {


        private readonly ApplicationDbContext Context;
        private readonly IMapper Mapper;
        private readonly UserManager<IdentityUser> UserManager;
        private readonly IAuthorizationService AuthorizationService;

        public PropertyController( ApplicationDbContext context, 
                IMapper mapper,
                UserManager<IdentityUser> userManager,
                IAuthorizationService authorizationService)
        {
            Context = context;
            Mapper = mapper;
            UserManager = userManager;
            AuthorizationService = authorizationService;
        }





        [HttpGet(Name = "getAllProperties")]
        [AllowAnonymous]
        [ServiceFilter(typeof(HATEOASPRopertyFilterAttribute))]

        public async Task<ActionResult<List<PropertyDTO>>> GetAll([FromQuery] PaginationDTO paginationDto)
        {

            var queryable = Context.Properties.AsQueryable();

            await HttpContext.InsertParamsPaginationInHeaders(queryable);




            //var properties = await Context.Properties
            var properties = await queryable
                            .Page(paginationDto)
                            .Include(x => x.Adviser)
                            .Include(x => x.User)
                            .Include(x => x.PropertyAmenity)
                            .ThenInclude(pa => pa.Amenity)
                            .ToListAsync();

            return  Mapper.Map<List<PropertyDTO>>(properties);
         

        }


        [HttpGet("{id:int}", Name = "getPropertyById")]
        [AllowAnonymous]
        // Aplicar nuestro Filter
        [ServiceFilter( typeof(HATEOASPRopertyFilterAttribute))]
        public async Task<ActionResult<PropertyDTO>> GetById(int id)
        {
            var property = await Context.Properties
                .Include(x => x.Adviser)
                .Include(x => x.User)
                .Include(x => x.PropertyAmenity )
                .ThenInclude( pa => pa.Amenity )
                .FirstOrDefaultAsync(x => x.Id == id);
        
            if ( property == null )
            {
                return NotFound($"No existe una propiead con el ID {id}");
            }

            var isAdmin = await AuthorizationService.AuthorizeAsync(User, "ADMIN");

            var propertyDto = Mapper.Map<PropertyDTO>(property);


            return propertyDto;

        }




        [HttpGet("{name}", Name = "getAllPropertiesByName")]
        [AllowAnonymous]
        public async Task<ActionResult<List<PropertyDTO>>> GetAllByName(string name, [FromQuery] PaginationDTO paginationDto)
        {

            var queryable = Context.Properties.AsQueryable();

            await HttpContext.InsertParamsPaginationInHeaders(queryable);


            var properties = await queryable
                     .Page(paginationDto)
                    .Include(x => x.Adviser)
                    .Include(x => x.User)
                    .Include(x => x.PropertyAmenity)
                    .ThenInclude(pa => pa.Amenity)
                    .Where(x => x.Name.Contains(name))
                    .ToListAsync();

            return Mapper.Map<List<PropertyDTO>>(properties);
        }




        [HttpPost(Name = "createProperty")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "USER")]
        public async Task<ActionResult> Create(PropertyCreateDTO propertyCreateDTO)
        {

            var email = HttpContext.User.Claims.Where(claim => claim.Type == "email").FirstOrDefault().Value;

            var user = await UserManager.FindByEmailAsync(email);

            var userId = user.Id;

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

            property.UserId = userId;

            Context.Add(property);

            await Context.SaveChangesAsync();

            var newProperty = await Context.Properties.AsNoTracking()
                    .Include(p => p.Adviser)
                    .Include(p => p.PropertyAmenity)
                    .ThenInclude(pa => pa.Amenity)
                    .FirstAsync(p => p.Id == property.Id);

            var propertyDTO = Mapper.Map<PropertyDTO>(newProperty);


            //return Ok();


            return CreatedAtRoute("getPropertyById", new { id = property.Id }, propertyDTO);

        }


        [HttpPut("{id:int}", Name = "updateProperty")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "USER")]
        public async Task<ActionResult> Update(int id, PropertyCreateDTO propertyCreateDTO)
        {
            var adviserdb = await Context.Properties
                    .Include(x => x.PropertyAmenity)
                    .FirstOrDefaultAsync(x => x.Id == id);

            if (adviserdb == null)
            {
                return NotFound($"No existe una propiead con el ID {id}");
            }

            // "Llevar las propiedades de propertyCreateDTO hacia adviserdb"
            // y asigno el resultado en la misma instancia adviserdb
            adviserdb = Mapper.Map(propertyCreateDTO, adviserdb);

            await Context.SaveChangesAsync();

            return NoContent();
        }



        [HttpPatch("{id:int}", Name = "patchProperty")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "USER")]
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

        /// <summary>
        /// Eliminar Propiedad
        /// </summary>
        /// <param name="id">Eliminar una propiedad por su ID</param>
        /// <returns></returns>

        [HttpDelete("{id:int}", Name = "deleteProperty")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "ADMIN")]
        public async Task<ActionResult> Delete(int id)
        {

            var exist = await Context.Properties.AnyAsync(x => x.Id == id);

            if ( !exist )
            {
                return NotFound($"No existe una propiead con el ID {id}");
            }

            Context.Remove(new Property() { Id = id });

            await Context.SaveChangesAsync();


            return NoContent();

        }


        



    }
}

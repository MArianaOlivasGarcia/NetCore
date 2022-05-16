using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiINMO.DTOs;
using WebApiINMO.Entities;
using WebApiINMO.Filters;

namespace WebApiINMO.Controllers.v1
{
    [ApiController]
    [Route("api/v1/advisers")]
    // [Authorize]
    public class AdviserController: ControllerBase
    {


        private readonly ApplicationDbContext Context;
        private readonly IMapper Mapper;
        private readonly IConfiguration Configuration;


        public AdviserController(ApplicationDbContext context, IMapper mapper, IConfiguration configuration)
        {
            Context = context;
            Mapper = mapper;
            Configuration = configuration;
        }



        [HttpGet(Name = "getAllAdvisersV1")]
        [ResponseCache(Duration = 10)]
        //[ServiceFilter(typeof(MyActionFilter))]
        public async Task<ActionResult<List<AdviserDTO>>> GetAll()
        {
            var advisers = await Context.Advisers.Include( x => x.Properties).ToListAsync();

            return Mapper.Map<List<AdviserDTO>>(advisers);
        }


        [HttpGet("{id:int}", Name = "getAdviserByIdV1")]
        public async Task<ActionResult<AdviserDTO>> GetById(int id)
        {
            var adviser = await Context.Advisers
                .Include(x => x.Properties)
                .FirstOrDefaultAsync(x => x.Id == id);

            if ( adviser == null )
            {
                return NotFound();
            }

            return Mapper.Map<AdviserDTO>(adviser);

        }


        [HttpGet("configV1")]
        public ActionResult<string> GetConfiguration()
        {
            //return Configuration["Lastname"];
            return Configuration["ConnectionStrings:DefaultConnection"];
        }



        [HttpPost(Name = "createAdviserV1")]
        //[Authorize]
        public async Task<ActionResult> Create([FromBody] AdviserCreateDTO adviserCreateDTO)
        {

            var exist = await Context.Advisers.AnyAsync( x => x.Name == adviserCreateDTO.Name );

            if ( exist )
            {
                return BadRequest($"Ya existe un asesor con el nombre { adviserCreateDTO.Name }");
            }

            var adviser = Mapper.Map<Adviser>(adviserCreateDTO);


            Context.Add(adviser);
            await Context.SaveChangesAsync();

            var adviserDTO = Mapper.Map<AdviserDTO>(adviser);

            // El nombre de la ruta y necesito mandarle el Id
            return CreatedAtRoute("getAdviserByIdV1", new { id = adviser.Id }, adviserDTO);
        }




        [HttpPut("{id:int}", Name = "updateAdviserV1")]
        public async Task<ActionResult> Update(AdviserCreateDTO adviserCreateDTO, int id)
        {
          
            var exist = await Context.Advisers.AnyAsync(x => x.Id == id);

            if (!exist)
            {
                return NotFound();
            }

            var adviser = Mapper.Map<Adviser>(adviserCreateDTO);
            adviser.Id = id;

            Context.Update(adviser);

            await Context.SaveChangesAsync();

            return NoContent();
        }




        [HttpDelete("{id:int}", Name = "deleteAdviserV1")]
        public async Task<ActionResult> Delete(int id)
        {

            var exist = await Context.Advisers.AnyAsync(x => x.Id == id);

            if (!exist)
            {
                return NotFound();
            }

            Context.Remove(new Adviser() { Id = id });
            await Context.SaveChangesAsync();
            return NoContent();
        }


    }
}

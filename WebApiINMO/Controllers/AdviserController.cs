using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiINMO.DTOs;
using WebApiINMO.Entities;
using WebApiINMO.Filters;

namespace WebApiINMO.Controllers
{
    [ApiController]
    [Route("api/advisers")]
    // [Authorize]
    public class AdviserController: ControllerBase
    {


        private readonly ApplicationDbContext Context;
        private readonly IMapper Mapper;


        public AdviserController(ApplicationDbContext context, IMapper mapper)
        {
            Context = context;
            Mapper = mapper;
        }



        [HttpGet]
        [ResponseCache(Duration = 10)]
        //[ServiceFilter(typeof(MyActionFilter))]
        public async Task<ActionResult<List<AdviserDTO>>> GetAll()
        {
            var advisers = await Context.Advisers.Include( x => x.Properties).ToListAsync();

            return Mapper.Map<List<AdviserDTO>>(advisers);
        }


        [HttpGet("{id:int}", Name = "GetAdviserById")]
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



        [HttpPost]
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
            return CreatedAtRoute("GetAdviserById", new { id = adviser.Id }, adviserDTO);
        }




        [HttpPut("{id:int}")]
        public async Task<ActionResult> Update(Adviser adviser, int id)
        {
            if (adviser.Id != id)
            {
                return BadRequest("El Id no coincide con el Id del asesor");
            }

            var exist = await Context.Advisers.AnyAsync(x => x.Id == id);

            if (!exist)
            {
                return NotFound();
            }

            Context.Update(adviser);
            await Context.SaveChangesAsync();
            return Ok();
        }




        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {

            var exist = await Context.Advisers.AnyAsync(x => x.Id == id);

            if (!exist)
            {
                return NotFound();
            }

            Context.Remove(new Adviser() { Id = id });
            await Context.SaveChangesAsync();
            return Ok();
        }


    }
}

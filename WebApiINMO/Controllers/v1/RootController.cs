using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApiINMO.DTOs;

namespace WebApiINMO.Controllers.v1
{
    [ApiController]
    [Route("api/v1")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RootController: ControllerBase 
    {

        private readonly IAuthorizationService AuthorizationService;

        public RootController(IAuthorizationService authorizationService)
        {
            AuthorizationService = authorizationService;
        }



        [HttpGet(Name = "getRoot")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<DataHATEOAS>>> Get()
        {
            var dataHateoas = new List<DataHATEOAS>();

            var isAdmin = await AuthorizationService.AuthorizeAsync(User, "ADMIN");

  

            dataHateoas.Add(new DataHATEOAS(
                Url: Url.Link("getRoot", new {}),
                Description: "self",
                Method: "Get"
            ));


            dataHateoas.Add(new DataHATEOAS(
                Url: Url.Link("getAllProperties", new { }),
                Description: "Obtener todos las propiedades",
                Method: "Get"
            ));


            dataHateoas.Add(new DataHATEOAS(
               Url: Url.Link("getAllPropertiesByName", new { name = "Algún nombre" }),
               Description: "Obtener todos las propiedades que coinciden con un nombre",
               Method: "Get"
           ));


            if ( isAdmin.Succeeded)
            {

                dataHateoas.Add(new DataHATEOAS(
                    Url: Url.Link("createProperty", new { }),
                    Description: "Crear una propiedad",
                    Method: "Post"
                ));

            }

            

            return dataHateoas;
        }

    }
}

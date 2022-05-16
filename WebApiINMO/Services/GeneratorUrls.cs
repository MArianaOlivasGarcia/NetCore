using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using WebApiINMO.DTOs;

namespace WebApiINMO.Services
{
    public class GeneratorUrls
    {

        private readonly IAuthorizationService AuthorizationService;
        private readonly IHttpContextAccessor HttpContextAccessor;
        private readonly IActionContextAccessor ActionContextAccessor;


        public GeneratorUrls(IAuthorizationService authorizationService, 
                            IHttpContextAccessor httpContextAccessor,
                            IActionContextAccessor actionContextAccessor) 
        {
            AuthorizationService = authorizationService;
            HttpContextAccessor = httpContextAccessor;
            ActionContextAccessor = actionContextAccessor;
        }





        private IUrlHelper CreateUrlHelper()
        {
            var factory = HttpContextAccessor.HttpContext.RequestServices.GetRequiredService<IUrlHelperFactory>();

            return factory.GetUrlHelper(ActionContextAccessor.ActionContext);
        }




        public async Task<bool> IsAdmin()
        {

            var httpContext = HttpContextAccessor.HttpContext;

            var result = await AuthorizationService.AuthorizeAsync(httpContext.User, "ADMIN");

            return result.Succeeded;
        }






        public async Task GenerateUrls(PropertyDTO propertyDto)
        {

            var isAdmin = await IsAdmin();

            var Url = CreateUrlHelper();

            propertyDto.Urls.Add(new DataHATEOAS(
                Url: Url.Link("getPropertyById", new { id = propertyDto.Id }),
                Description: "Obtener una propíedad por ID",
                Method: "GET"
                ));

            if (isAdmin)
            {

                propertyDto.Urls.Add(new DataHATEOAS(
                    Url: Url.Link("updateProperty", new { id = propertyDto.Id }),
                    Description: "Actualizar una propiedad por ID",
                    Method: "PUT"
                    ));

                propertyDto.Urls.Add(new DataHATEOAS(
                    Url: Url.Link("deleteProperty", new { id = propertyDto.Id }),
                    Description: "Borrar una propiedad por ID",
                    Method: "DELETE"
                    ));
            }
        }


    }
}

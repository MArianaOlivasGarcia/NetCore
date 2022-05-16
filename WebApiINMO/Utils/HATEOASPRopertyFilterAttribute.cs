using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebApiINMO.DTOs;
using WebApiINMO.Services;

namespace WebApiINMO.Utils
{
    public class HATEOASPRopertyFilterAttribute: HATEOASFilterAttribute
    {

        private readonly GeneratorUrls GeneratorUrls;

        public HATEOASPRopertyFilterAttribute(GeneratorUrls generatorUrls)
        {
            GeneratorUrls = generatorUrls;
        }

        public override async Task OnResultExecutionAsync( ResultExecutingContext context, ResultExecutionDelegate next ) 
        {

            var include = IncludeHATEOAS(context);

            if ( !include )
            {
                await next();
                return;
            }


            var result = context.Result as ObjectResult;

            //var model = result.Value as PropertyDTO ?? throw new ArgumentNullException("Se esperaba una instancia de PropertyDTO");

            var propertyDTO = result.Value as PropertyDTO;

            if ( propertyDTO == null )
            {
                // Verificar si entonces es una lista
                var propertiesDTO = result.Value as List<PropertyDTO> ?? throw new ArgumentNullException("Se esperaba una instancia de PropertyDTO o List<PropertyDTO>");

                propertiesDTO.ForEach(async property => await GeneratorUrls.GenerateUrls(property));

                result.Value = propertiesDTO;
            }
            else
            {
                await GeneratorUrls.GenerateUrls(propertyDTO);
            }

            await next();



        }


    }
}

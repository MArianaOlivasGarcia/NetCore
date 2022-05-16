using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WebApiINMO.Utils
{
    public class AddParamsHATEOAS: IOperationFilter
    {


        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {

            if ( context.ApiDescription.HttpMethod != "GET") { return; }

            if (operation.Parameters == null)
            {
                operation.Parameters = new List<OpenApiParameter>();

            }

            operation.Parameters.Add(new OpenApiParameter
            {
                    Name = "includeHATEOAS",
                    In = ParameterLocation.Header,
                    Required = false
            });
        }
    }
}

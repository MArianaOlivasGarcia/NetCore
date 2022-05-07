using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApiINMO.Filters
{
    public class MyActionFilter : IActionFilter
    {
        public readonly ILogger<MyActionFilter> Logger;

        public MyActionFilter(ILogger<MyActionFilter> logger)
        {
            Logger = logger;
        }
            

        //  Cuando se esta ejecutando la acción
        public void OnActionExecuting(ActionExecutingContext context)
        {
            Logger.LogInformation("Antes de ejecutar la acción");
        }

        // Cuando ya se ejecuto
        public void OnActionExecuted(ActionExecutedContext context)
        {
            Logger.LogInformation("Despues de ejecutar la acción");
        }


    }
}

using Microsoft.EntityFrameworkCore;

namespace WebApiINMO.Utils
{
    public static class HttpContextExtensions
    {


        public async static Task InsertParamsPaginationInHeaders<T>( this HttpContext httpContext, IQueryable<T> queryable )
        {
            if ( httpContext == null )
            {
                throw new ArgumentNullException(nameof(httpContext));
            }


            double totalResult = await queryable.CountAsync();

            httpContext.Response.Headers.Add("totalResults", totalResult.ToString());

        }




    }
}

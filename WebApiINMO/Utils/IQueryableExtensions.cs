using WebApiINMO.DTOs;

namespace WebApiINMO.Utils
{
    public static class IQueryableExtensions
    {


        public static IQueryable<T> Page<T>(this IQueryable<T> queryable, PaginationDTO paginationDto)
        {


            return queryable
                    .Skip((paginationDto.Page - 1 ) * paginationDto.Limit)
                    .Take(paginationDto.Limit);
        }


    }
}

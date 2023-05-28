using Microsoft.EntityFrameworkCore;
using Roaa.Rosas.Common.Models;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Common.Extensions
{
    public static class EFCorePaginatorExtension
    {
        public static PaginatedResult<TSource> ToPagedResult<TSource>(this IEnumerable<TSource> source, int? page, int? pageSize)
        {
            var count = source.Count();
            var pagedData = source.ToPage(page, pageSize).ToList();
            return PaginatedResult<TSource>.Successful(pagedData, count);
        }


        public static async Task<PaginatedResult<TSource>> ToPagedResultAsync<TSource>(this IQueryable<TSource> source, PaginationMetaData paginationInfo)
        {
            return await source.ToPagedResultAsync(paginationInfo?.Page, paginationInfo?.PageSize);
        }


        public static async Task<PaginatedResult<TSource>> ToPagedResultAsync<TSource>(this IQueryable<TSource> source, int? page, int? pageSize)
        {
            var count = await source.CountAsync();

            var pagedData = await source.ToPage(page, pageSize).ToListAsync();
            return PaginatedResult<TSource>.Successful(pagedData, count);
        }


        public static IQueryable<TSource> ToPage<TSource>(this IQueryable<TSource> source, int? page, int? pageSize)
        {
            int p = page ?? 1;
            p = p == 0 ? 1 : p;
            int pz = pageSize ?? 10;
            return source.Skip((p - 1) * pz).Take(pz);
        }


        public static IEnumerable<TSource> ToPage<TSource>(this IEnumerable<TSource> source, int? page, int? pageSize)
        {
            int p = page ?? 1;
            p = p == 0 ? 1 : p;
            int pz = pageSize ?? 10;
            return source.Skip((p - 1) * pz).Take(pz);
        }
    }
}

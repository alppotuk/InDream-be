using InDream.Core.Pagination;
using Microsoft.EntityFrameworkCore;

namespace InDream.Core.Repository.Extensions;
public static class QueryableExtensions
{
    public static async Task<(List<T> data, PaginationResult paginationResult)> ToPagerAsync<T>(
        this IQueryable<T> query,
        PaginationFilter filter)
    {
        var totalRecords = await query.CountAsync();

        var pagedData = await query
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        var paginationResult = new PaginationResult(filter);

        paginationResult.TotalRecords = totalRecords;
        paginationResult.TotalPages = (int)Math.Ceiling(totalRecords / (double)paginationResult.PageSize);
        paginationResult.HasPreviousPage = paginationResult.PageNumber > 1;
        paginationResult.HasNextPage = paginationResult.PageNumber < paginationResult.TotalPages;

        return (pagedData, paginationResult);
    }
}
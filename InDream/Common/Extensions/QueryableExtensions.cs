using InDream.Interfaces;
using Microsoft.EntityFrameworkCore;

public static class QueryableExtensions
{
    public static async Task<(List<T> data, Pagination pagination)> ToPagerAsync<T>(
        this IQueryable<T> query,
        PaginationFilter filter)
    {
        var totalRecords = await query.CountAsync();

        var pagedData = await query
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        var pagination = new Pagination(filter);

        pagination.TotalRecords = totalRecords;
        pagination.TotalPages = (int)Math.Ceiling(totalRecords / (double)pagination.PageSize);
        pagination.HasPreviousPage = pagination.PageNumber > 1;
        pagination.HasNextPage = pagination.PageNumber < pagination.TotalPages;

        return (pagedData, pagination);
    }
}
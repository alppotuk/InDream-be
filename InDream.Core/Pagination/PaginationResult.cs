namespace InDream.Core.Pagination;

public class PaginationResult
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public int TotalRecords { get; set; }
    public bool HasPreviousPage { get; set; }
    public bool HasNextPage { get; set; }


    public PaginationResult(PaginationFilter filter)
    {
        PageNumber = filter.PageNumber;
        PageSize = filter.PageSize;
    }

}

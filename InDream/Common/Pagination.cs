namespace InDream.Interfaces
{
    public class Pagination
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }


        public Pagination(PaginationFilter filter)
        {
            PageNumber = filter.PageNumber;
            PageSize = filter.PageSize;
        }
   
    }
}

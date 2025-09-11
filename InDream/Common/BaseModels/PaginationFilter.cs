namespace InDream.Common.BaseModels
{
    public class PaginationFilter
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        

        public PaginationFilter()
        {
            PageNumber = 1;
            PageSize = 10;
        }
    }
}

namespace InDream.Interfaces
{
    public class PaginationFilter
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        

        public PaginationFilter()
        {
            this.PageNumber = 1;
            this.PageSize = 10;
        }
    }
}

namespace InDream.Interfaces
{
    public class Response<T>
    {
        public bool Success { get; set; }
        public T Data { get; set; }
        public Pagination? Pagination { get; set; }

        public Response(bool success, T data, Pagination? pagination = null)
        {
            Success = success;
            Data = data;
            Pagination = pagination;
        }
    }
}

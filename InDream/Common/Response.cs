namespace InDream.Interfaces
{
    public class ResponseBase<T>
    {
        public bool Success { get; set; }
        public T Data { get; set; }
        public string? Message { get; set; }
        public Pagination? Pagination { get; set; }

        public ResponseBase(bool success, T data, Pagination? pagination = null, string? message = null)
        {
            Success = success;
            Data = data;
            Pagination = pagination;
            Message = Message;
        }
    }
}

using InDream.Core.Pagination;

namespace InDream.Core.BaseModels
{
    public class ResponseBase<T>
    {
        public bool Success { get; set; }
        public T Data { get; set; }
        public string? Message { get; set; }
        public PaginationResult? PaginationResult { get; set; }

        public ResponseBase(bool success, T data, PaginationResult? paginationResult = null, string? message = null)
        {
            Success = success;
            Data = data;
            PaginationResult = paginationResult;
            Message = Message;
        }
    }
}

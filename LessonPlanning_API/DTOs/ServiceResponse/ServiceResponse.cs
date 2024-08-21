namespace Lesson_API.DTOs.ServiceResponse
{
    public class ServiceResponse<T>
    {
        public T Data { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
        public int StatusCode { get; set; }  // Add this property
        public int? TotalCount { get; set; }  // Add this property

        public ServiceResponse(T data, bool success, string message, int statusCode, int? totalCount = null)
        {
            Data = data;
            Success = success;
            Message = message;
            StatusCode = statusCode;
            TotalCount = totalCount;
        }
    }
}

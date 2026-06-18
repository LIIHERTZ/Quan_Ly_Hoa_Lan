namespace QuanLyHoaLan.Application.Common.Models;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? ErrorCode { get; set; }
    public IDictionary<string, string[]>? ValidationErrors { get; set; }
    public string? Details { get; set; }
    public DateTime Timestamp { get; set; }

    public static ApiResponse<T> Ok(T data, string message = "Thành công")
    {
        return new ApiResponse<T>
        {
            Success = true,
            Data = data,
            Message = message,
            Timestamp = DateTime.UtcNow
        };
    }

    public static ApiResponse<T> Fail(string message, string? errorCode = null, IDictionary<string, string[]>? validationErrors = null, string? details = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Data = default,
            Message = message,
            ErrorCode = errorCode,
            ValidationErrors = validationErrors,
            Details = details,
            Timestamp = DateTime.UtcNow
        };
    }
}

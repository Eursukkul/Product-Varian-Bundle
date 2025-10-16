namespace FlowAccount.Application.DTOs.Common;

/// <summary>
/// Generic API Response wrapper
/// </summary>
public class ResponseDto<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    public List<string>? Errors { get; set; }

    public static ResponseDto<T> SuccessResponse(T data, string? message = null)
    {
        return new ResponseDto<T>
        {
            Success = true,
            Message = message ?? "Operation completed successfully",
            Data = data
        };
    }

    public static ResponseDto<T> ErrorResponse(string message, List<string>? errors = null)
    {
        return new ResponseDto<T>
        {
            Success = false,
            Message = message,
            Errors = errors
        };
    }
}

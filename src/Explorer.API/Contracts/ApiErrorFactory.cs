using Microsoft.AspNetCore.Http;

namespace Explorer.API.Contracts;

public static class ApiErrorFactory
{
    public static ApiErrorResponse Create(HttpContext context, string errorCode, string message, string? details = null)
    {
        return new ApiErrorResponse
        {
            ErrorCode = errorCode,
            Message = message,
            Details = details,
            TraceId = context.TraceIdentifier
        };
    }
}

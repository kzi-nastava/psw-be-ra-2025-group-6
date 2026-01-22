using System.Text.Json.Serialization;

namespace Explorer.API.Contracts;

public class ApiErrorResponse
{
    [JsonPropertyName("errorCode")]
    public string ErrorCode { get; init; } = string.Empty;

    [JsonPropertyName("message")]
    public string Message { get; init; } = string.Empty;

    [JsonPropertyName("details")]
    public string? Details { get; init; }

    [JsonPropertyName("traceId")]
    public string? TraceId { get; init; }
}

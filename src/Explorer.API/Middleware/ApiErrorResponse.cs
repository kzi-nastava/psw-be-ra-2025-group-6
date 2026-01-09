using System.Text.Json.Serialization;

namespace Explorer.API.Middleware;

public class ApiErrorResponse
{
    [JsonPropertyName("code")]
    public string Code { get; }

    [JsonPropertyName("message")]
    public string Message { get; }

    [JsonPropertyName("errors")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IReadOnlyCollection<ApiErrorDetail>? Errors { get; }

    public ApiErrorResponse(string code, string message, IReadOnlyCollection<ApiErrorDetail>? errors = null)
    {
        Code = code;
        Message = message;
        Errors = errors;
    }
}

public class ApiErrorDetail
{
    [JsonPropertyName("field")]
    public string Field { get; }

    [JsonPropertyName("message")]
    public string Message { get; }

    public ApiErrorDetail(string field, string message)
    {
        Field = field;
        Message = message;
    }
}

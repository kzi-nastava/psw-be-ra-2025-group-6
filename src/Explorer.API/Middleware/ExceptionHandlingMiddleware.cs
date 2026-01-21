using Explorer.API.Contracts;
using Explorer.BuildingBlocks.Core.Exceptions;
using System.Net;
using System.Text.Json;

namespace Explorer.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, errorCode, message, details) = exception switch
        {
            ArgumentException ex => (HttpStatusCode.BadRequest, ApiErrorCodes.ValidationError, "Request validation failed.", ex.Message),
            UnauthorizedAccessException ex => (HttpStatusCode.Unauthorized, ApiErrorCodes.AuthRequired, "Login required to perform this action.", ex.Message),
            ForbiddenException ex => (HttpStatusCode.Forbidden, ApiErrorCodes.Forbidden, "You don't have permission for this action.", ex.Message),
            NotFoundException ex => (HttpStatusCode.NotFound, ApiErrorCodes.NotFound, "Requested resource not found.", ex.Message),
            EntityValidationException ex => (HttpStatusCode.UnprocessableEntity, ApiErrorCodes.ValidationError, "Request validation failed.", ex.Message),
            InvalidOperationException ex => (HttpStatusCode.Conflict, ApiErrorCodes.Conflict, "Operation is not allowed in the current state.", ex.Message),
            _ => (HttpStatusCode.InternalServerError, ApiErrorCodes.ServerError, "Server error. Try later.", exception.Message)
        };

        context.Response.StatusCode = (int)statusCode;

        var response = new ApiErrorResponse
        {
            ErrorCode = errorCode,
            Message = message,
            Details = details,
            TraceId = context.TraceIdentifier
        };

        var jsonResponse = JsonSerializer.Serialize(response);
        await context.Response.WriteAsync(jsonResponse);
    }
}

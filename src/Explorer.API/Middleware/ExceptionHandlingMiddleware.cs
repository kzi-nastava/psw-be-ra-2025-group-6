using Explorer.BuildingBlocks.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
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

        var (statusCode, response) = exception switch
        {
            RequestValidationException ex => (
                HttpStatusCode.BadRequest,
                new ApiErrorResponse(
                    "VALIDATION_ERROR",
                    "Validation failed.",
                    ex.Errors.Select(e => new ApiErrorDetail(e.Field, e.Message)).ToList()
                )
            ),
            AlreadyExistsException ex => (
                HttpStatusCode.Conflict,
                new ApiErrorResponse(
                    "CONFLICT",
                    "Resource already exists.",
                    new List<ApiErrorDetail>
                    {
                        new(ex.Field ?? "general", ex.Message)
                    }
                )
            ),
            EntityValidationException ex => (
                HttpStatusCode.BadRequest,
                new ApiErrorResponse(
                    "VALIDATION_ERROR",
                    "Validation failed.",
                    new List<ApiErrorDetail>
                    {
                        new("general", ex.Message)
                    }
                )
            ),
            ArgumentException ex => (
                HttpStatusCode.BadRequest,
                new ApiErrorResponse(
                    "VALIDATION_ERROR",
                    "Validation failed.",
                    new List<ApiErrorDetail>
                    {
                        new("general", ex.Message)
                    }
                )
            ),
            UnauthorizedAccessException ex => BuildUnauthorizedResponse(context, ex),
            ForbiddenException ex => (
                HttpStatusCode.Forbidden,
                new ApiErrorResponse("FORBIDDEN", ex.Message)
            ),
            NotFoundException ex => (
                HttpStatusCode.NotFound,
                new ApiErrorResponse("NOT_FOUND", ex.Message)
            ),
            _ => (
                HttpStatusCode.InternalServerError,
                new ApiErrorResponse("INTERNAL_ERROR", "An unexpected error occurred.")
            )
        };

        context.Response.StatusCode = (int)statusCode;

        var jsonResponse = JsonSerializer.Serialize(response);
        await context.Response.WriteAsync(jsonResponse);
    }

    private static (HttpStatusCode, ApiErrorResponse) BuildUnauthorizedResponse(HttpContext context, UnauthorizedAccessException ex)
    {
        var isLogin = context.Request.Path.StartsWithSegments("/api/users/login", StringComparison.OrdinalIgnoreCase);
        var isInvalidCredentials = ex.Message.Contains("Invalid credentials", StringComparison.OrdinalIgnoreCase);

        if (isLogin || isInvalidCredentials)
        {
            return (HttpStatusCode.Unauthorized, new ApiErrorResponse("INVALID_CREDENTIALS", "Invalid username or password."));
        }

        return (HttpStatusCode.Forbidden, new ApiErrorResponse("FORBIDDEN", ex.Message));
    }
}

using Ambev.DeveloperEvaluation.WebApi.Common;
using FluentValidation;
using System.Net;
using System.Text.Json;

namespace Ambev.DeveloperEvaluation.WebApi.Middleware;

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
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, response) = exception switch
        {
            ValidationException validationEx => HandleValidationException(validationEx),
            InvalidOperationException invalidOpEx => HandleInvalidOperationException(invalidOpEx),
            KeyNotFoundException notFoundEx => HandleNotFoundException(notFoundEx),
            UnauthorizedAccessException unauthorizedEx => HandleUnauthorizedException(unauthorizedEx),
            _ => HandleGenericException(exception)
        };

        // Log the exception
        LogException(exception, statusCode);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, jsonOptions));
    }

    private (int StatusCode, ErrorResponse Response) HandleValidationException(ValidationException ex)
    {
        var errors = ex.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.ErrorMessage).ToArray());

        return ((int)HttpStatusCode.BadRequest, new ErrorResponse
        {
            Type = "ValidationError",
            Title = "One or more validation errors occurred",
            Status = (int)HttpStatusCode.BadRequest,
            Errors = errors
        });
    }

    private (int StatusCode, ErrorResponse Response) HandleInvalidOperationException(InvalidOperationException ex)
    {
        return ((int)HttpStatusCode.BadRequest, new ErrorResponse
        {
            Type = "BusinessRuleViolation",
            Title = "Business rule violation",
            Status = (int)HttpStatusCode.BadRequest,
            Detail = ex.Message
        });
    }

    private (int StatusCode, ErrorResponse Response) HandleNotFoundException(KeyNotFoundException ex)
    {
        return ((int)HttpStatusCode.NotFound, new ErrorResponse
        {
            Type = "ResourceNotFound",
            Title = "Resource not found",
            Status = (int)HttpStatusCode.NotFound,
            Detail = ex.Message
        });
    }

    private (int StatusCode, ErrorResponse Response) HandleUnauthorizedException(UnauthorizedAccessException ex)
    {
        return ((int)HttpStatusCode.Unauthorized, new ErrorResponse
        {
            Type = "Unauthorized",
            Title = "Unauthorized access",
            Status = (int)HttpStatusCode.Unauthorized,
            Detail = ex.Message
        });
    }

    private (int StatusCode, ErrorResponse Response) HandleGenericException(Exception ex)
    {
        return ((int)HttpStatusCode.InternalServerError, new ErrorResponse
        {
            Type = "InternalServerError",
            Title = "An unexpected error occurred",
            Status = (int)HttpStatusCode.InternalServerError,
            Detail = "An internal server error occurred. Please try again later."
        });
    }

    private void LogException(Exception exception, int statusCode)
    {
        var logLevel = statusCode >= 500 ? LogLevel.Error : LogLevel.Warning;

        _logger.Log(
            logLevel,
            exception,
            "Exception occurred | Type: {ExceptionType} | Message: {Message} | StatusCode: {StatusCode}",
            exception.GetType().Name,
            exception.Message,
            statusCode);
    }
}

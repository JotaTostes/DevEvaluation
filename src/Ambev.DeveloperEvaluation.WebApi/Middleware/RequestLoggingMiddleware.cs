using System.Diagnostics;
using System.Text;

namespace Ambev.DeveloperEvaluation.WebApi.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    /// <summary>
    /// Initializes a new instance of RequestLoggingMiddleware
    /// </summary>
    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Invokes the middleware
    /// </summary>
    public async Task InvokeAsync(HttpContext context)
    {
        var requestId = Guid.NewGuid().ToString("N")[..8];
        var stopwatch = Stopwatch.StartNew();

        await LogRequest(context, requestId);

        var originalBodyStream = context.Response.Body;

        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();

            await LogResponse(context, requestId, stopwatch.ElapsedMilliseconds);

            await responseBody.CopyToAsync(originalBodyStream);
        }
    }

    private async Task LogRequest(HttpContext context, string requestId)
    {
        context.Request.EnableBuffering();

        var requestBody = string.Empty;

        if (context.Request.ContentLength > 0)
        {
            using var reader = new StreamReader(
                context.Request.Body,
                Encoding.UTF8,
                detectEncodingFromByteOrderMarks: false,
                leaveOpen: true);

            requestBody = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0;
        }

        _logger.LogInformation(
            "HTTP Request {RequestId} | {Method} {Path}{QueryString} | " +
            "ContentType: {ContentType} | ContentLength: {ContentLength} | " +
            "Body: {Body}",
            requestId,
            context.Request.Method,
            context.Request.Path,
            context.Request.QueryString,
            context.Request.ContentType ?? "N/A",
            context.Request.ContentLength ?? 0,
            TruncateBody(requestBody));
    }

    private async Task LogResponse(HttpContext context, string requestId, long elapsedMs)
    {
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
        context.Response.Body.Seek(0, SeekOrigin.Begin);

        var logLevel = context.Response.StatusCode >= 400 ? LogLevel.Warning : LogLevel.Information;

        _logger.Log(
            logLevel,
            "HTTP Response {RequestId} | {StatusCode} | Duration: {Duration}ms | " +
            "ContentType: {ContentType} | Body: {Body}",
            requestId,
            context.Response.StatusCode,
            elapsedMs,
            context.Response.ContentType ?? "N/A",
            TruncateBody(responseBody));
    }

    private static string TruncateBody(string body, int maxLength = 1000)
    {
        if (string.IsNullOrEmpty(body))
            return "[empty]";

        return body.Length > maxLength
            ? $"{body[..maxLength]}... [truncated]"
            : body;
    }
}

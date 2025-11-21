namespace Ambev.DeveloperEvaluation.WebApi.Middleware;

public static class MiddlewareExtensions
{
    /// <summary>
    /// Adds the request logging middleware to the application pipeline
    /// </summary>
    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder app)
    {
        return app.UseMiddleware<RequestLoggingMiddleware>();
    }

    /// <summary>
    /// Adds the exception handling middleware to the application pipeline
    /// </summary>
    public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}

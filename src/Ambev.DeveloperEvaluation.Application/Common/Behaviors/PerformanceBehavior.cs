using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Application.Common.Behaviors;

public class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger;
    private readonly Stopwatch _timer;

    /// <summary>
    /// Initializes a new instance of PerformanceBehavior
    /// </summary>
    public PerformanceBehavior(ILogger<PerformanceBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
        _timer = new Stopwatch();
    }

    /// <summary>
    /// Handles the request and logs performance metrics
    /// </summary>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        _logger.LogDebug(
            "Handling {RequestName} | Request: {@Request}",
            requestName,
            request);

        _timer.Start();

        var response = await next();

        _timer.Stop();

        var elapsedMilliseconds = _timer.ElapsedMilliseconds;

        if (elapsedMilliseconds > 500)
        {
            _logger.LogWarning(
                "Long running request | {RequestName} | Duration: {ElapsedMilliseconds}ms | Request: {@Request}",
                requestName,
                elapsedMilliseconds,
                request);
        }
        else
        {
            _logger.LogInformation(
                "Completed {RequestName} | Duration: {ElapsedMilliseconds}ms",
                requestName,
                elapsedMilliseconds);
        }

        return response;
    }
}

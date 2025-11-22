using Ambev.DeveloperEvaluation.Domain.Services;
using Microsoft.Extensions.Logging;
using Rebus.Bus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.ORM.EventPublisher;

public class RebusEventPublisher : IEventPublisher
{
    private readonly IBus? _bus;
    private readonly ILogger<RebusEventPublisher> _logger;

    /// <summary>
    /// Initializes a new instance of RebusEventPublisher
    /// </summary>
    /// <param name="bus">The Rebus bus instance (optional)</param>
    /// <param name="logger">The logger instance</param>
    public RebusEventPublisher(IBus? bus, ILogger<RebusEventPublisher> logger)
    {
        _bus = bus;
        _logger = logger;
    }

    /// <summary>
    /// Publishes an event using Rebus or logs it if Rebus is not available
    /// </summary>
    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : class
    {
        try
        {
            if (_bus != null)
            {
                await _bus.Publish(@event);
                _logger.LogInformation(
                    "Event {EventType} published to message bus. Event: {EventData}",
                    typeof(TEvent).Name,
                    JsonSerializer.Serialize(@event));
            }
            else
            {
                _logger.LogInformation(
                    "Event {EventType} logged (Rebus not configured). Event: {EventData}",
                    typeof(TEvent).Name,
                    JsonSerializer.Serialize(@event));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error publishing event {EventType}. Event: {EventData}",
                typeof(TEvent).Name,
                JsonSerializer.Serialize(@event));
            throw;
        }
    }
}

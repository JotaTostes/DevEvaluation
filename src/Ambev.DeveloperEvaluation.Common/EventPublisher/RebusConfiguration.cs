using Ambev.DeveloperEvaluation.Domain.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rebus.Config;
using Rebus.Routing.TypeBased;
using Rebus.Transport.InMem;

namespace Ambev.DeveloperEvaluation.Common.EventPublisher;

public static class RebusConfiguration
{
    /// <summary>
    /// Adds Rebus configuration to the service collection
    /// </summary>
    /// <param name="services">The service collection</param
    /// <param name="configuration">The configuration instance</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddRebusConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var rebusEnabled = configuration.GetValue<bool>("Rebus:Enabled");

        if (rebusEnabled)
        {
            var connectionString = configuration.GetValue<string>("Rebus:ConnectionString");
            var queueName = configuration.GetValue<string>("Rebus:QueueName") ?? "sales-events";

            services.AddRebus(configure => configure
                .Logging(l => l.Serilog())
                .Transport(t => t.UseInMemoryTransport(new InMemNetwork(), queueName))
                .Routing(r => r.TypeBased()
                    .MapAssemblyOf<SaleCreatedEvent>(queueName)));

            // Start the bus
            services.AddRebusHandler<SaleEventsHandler>();
        }

        return services;
    }
}

/// <summary>
/// Handler for sale events (example handler for demonstration)
/// In a real scenario, this would be in a separate consumer service
/// </summary>
public class SaleEventsHandler :
    Rebus.Handlers.IHandleMessages<Domain.Events.SaleCreatedEvent>,
    Rebus.Handlers.IHandleMessages<Domain.Events.SaleModifiedEvent>,
    Rebus.Handlers.IHandleMessages<Domain.Events.SaleCancelledEvent>,
    Rebus.Handlers.IHandleMessages<Domain.Events.ItemCancelledEvent>
{
    private readonly Microsoft.Extensions.Logging.ILogger<SaleEventsHandler> _logger;

    public SaleEventsHandler(Microsoft.Extensions.Logging.ILogger<SaleEventsHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(Domain.Events.SaleCreatedEvent message)
    {
        _logger.LogInformation(
            "Received SaleCreatedEvent - SaleId: {SaleId}, SaleNumber: {SaleNumber}, TotalAmount: {TotalAmount}",
            message.SaleId, message.SaleNumber, message.TotalAmount);
        return Task.CompletedTask;
    }

    public Task Handle(Domain.Events.SaleModifiedEvent message)
    {
        _logger.LogInformation(
            "Received SaleModifiedEvent - SaleId: {SaleId}, Previous: {Previous}, New: {New}",
            message.SaleId, message.PreviousTotalAmount, message.NewTotalAmount);
        return Task.CompletedTask;
    }

    public Task Handle(Domain.Events.SaleCancelledEvent message)
    {
        _logger.LogInformation(
            "Received SaleCancelledEvent - SaleId: {SaleId}, SaleNumber: {SaleNumber}",
            message.SaleId, message.SaleNumber);
        return Task.CompletedTask;
    }

    public Task Handle(Domain.Events.ItemCancelledEvent message)
    {
        _logger.LogInformation(
            "Received ItemCancelledEvent - ItemId: {ItemId}, SaleId: {SaleId}, Product: {ProductName}",
            message.ItemId, message.SaleId, message.ProductName);
        return Task.CompletedTask;
    }

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rebus.Config;
using Rebus.Routing.TypeBased;
using Rebus.Transport.InMem;
using Rebus.ServiceProvider;

namespace Ambev.DeveloperEvaluation.Common.EventPublisher;

public static class RebusConfiguration
{
    public static IServiceCollection AddRebusConfiguration<TEventAssemblyMarker>(
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
                    .MapAssemblyOf<TEventAssemblyMarker>(queueName)));

            services.AutoRegisterHandlersFromAssemblyOf<TEventAssemblyMarker>();
        }

        return services;
    }
}
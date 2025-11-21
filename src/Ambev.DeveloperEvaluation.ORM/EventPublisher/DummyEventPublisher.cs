using Ambev.DeveloperEvaluation.Domain.Services;


namespace Ambev.DeveloperEvaluation.ORM.EventPublisher;

public class DummyEventPublisher : IEventPublisher
{
    public Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : class
    {
        return Task.CompletedTask;
    }
}

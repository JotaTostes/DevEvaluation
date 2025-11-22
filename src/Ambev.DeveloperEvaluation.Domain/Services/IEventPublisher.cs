namespace Ambev.DeveloperEvaluation.Domain.Services;

public interface IEventPublisher
{
    /// <summary>
    /// Publishes an event to the message bus
    /// </summary>
    /// <typeparam name="TEvent">The type of event to publish</typeparam>
    /// <param name="event">The event to publish</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : class;
}

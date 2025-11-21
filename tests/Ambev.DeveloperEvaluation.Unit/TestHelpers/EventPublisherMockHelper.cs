using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Services;
using NSubstitute;

namespace Ambev.DeveloperEvaluation.Unit.TestHelpers;

public static class EventPublisherMockHelper
{
    /// <summary>
    /// Creates a mock IEventPublisher with default setup
    /// </summary>
    public static IEventPublisher CreateEventPublisherMock()
    {
        var publisher = Substitute.For<IEventPublisher>();

        publisher.PublishAsync(Arg.Any<object>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        return publisher;
    }

    /// <summary>
    /// Verifies that SaleCreatedEvent was published
    /// </summary>
    public static async Task VerifySaleCreatedEventPublished(
        this IEventPublisher publisher,
        Guid saleId)
    {
        await publisher.Received(1).PublishAsync(
            Arg.Is<SaleCreatedEvent>(e => e.SaleId == saleId),
            Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Verifies that SaleModifiedEvent was published
    /// </summary>
    public static async Task VerifySaleModifiedEventPublished(
        this IEventPublisher publisher,
        Guid saleId)
    {
        await publisher.Received(1).PublishAsync(
            Arg.Is<SaleModifiedEvent>(e => e.SaleId == saleId),
            Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Verifies that SaleCancelledEvent was published
    /// </summary>
    public static async Task VerifySaleCancelledEventPublished(
        this IEventPublisher publisher,
        Guid saleId)
    {
        await publisher.Received(1).PublishAsync(
            Arg.Is<SaleCancelledEvent>(e => e.SaleId == saleId),
            Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Verifies that ItemCancelledEvent was published
    /// </summary>
    public static async Task VerifyItemCancelledEventPublished(
        this IEventPublisher publisher,
        Guid itemId)
    {
        await publisher.Received(1).PublishAsync(
            Arg.Is<ItemCancelledEvent>(e => e.ItemId == itemId),
            Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Verifies that no events were published
    /// </summary>
    public static async Task VerifyNoEventsPublished(this IEventPublisher publisher)
    {
        await publisher.DidNotReceive().PublishAsync(
            Arg.Any<object>(),
            Arg.Any<CancellationToken>());
    }
}
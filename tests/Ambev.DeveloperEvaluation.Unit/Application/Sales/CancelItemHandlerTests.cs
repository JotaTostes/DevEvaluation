using Ambev.DeveloperEvaluation.Application.Sales.CancelItem;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Unit.Application.TestData;
using Ambev.DeveloperEvaluation.Unit.TestHelpers;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class CancelItemHandlerTests : TestBase
{
    private readonly SaleFaker _saleFaker = new();

    [Fact]
    public async Task Handle_WithValidItem_ShouldCancelItemAndPublishEvent()
    {
        // Arrange
        var existingSale = _saleFaker.GenerateWithItems(3);
        var itemToCancel = existingSale.Items.First();
        SetupSaleRepository(existingSale);
        SaleRepository.SetupUpdateAsync();

        var command = new CancelItemCommand(existingSale.Id, itemToCancel.Id);
        var handler = new CancelItemHandler(SaleRepository, Mapper, EventPublisher);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var cancelledItem = result.Items.First(i => i.Id == itemToCancel.Id);
        cancelledItem.IsCancelled.Should().BeTrue();
        cancelledItem.CancelledAt.Should().NotBeNull();

        await SaleRepository.Received(1).UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
        await EventPublisher.Received(1).PublishAsync(
            Arg.Any<ItemCancelledEvent>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithNonExistentSale_ShouldThrowException()
    {
        // Arrange
        SetupSaleRepositoryNotFound();
        var saleId = Guid.NewGuid();
        var itemId = Guid.NewGuid();
        var command = new CancelItemCommand(saleId, itemId);
        var handler = new CancelItemHandler(SaleRepository, Mapper, EventPublisher);

        // Act
        var act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"*Sale with ID {saleId} not found*");
    }

    [Fact]
    public async Task Handle_WithNonExistentItem_ShouldThrowException()
    {
        // Arrange
        var existingSale = _saleFaker.GenerateWithItems();
        SetupSaleRepository(existingSale);

        var nonExistentItemId = Guid.NewGuid();
        var command = new CancelItemCommand(existingSale.Id, nonExistentItemId);
        var handler = new CancelItemHandler(SaleRepository, Mapper, EventPublisher);

        // Act
        var act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"*Item with ID {nonExistentItemId} not found in sale*");
    }

    [Fact]
    public async Task Handle_WithCancelledSale_ShouldThrowException()
    {
        // Arrange
        var cancelledSale = _saleFaker.GenerateCancelled();
        var itemId = cancelledSale.Items.First().Id;
        SetupSaleRepository(cancelledSale);

        var command = new CancelItemCommand(cancelledSale.Id, itemId);
        var handler = new CancelItemHandler(SaleRepository, Mapper, EventPublisher);

        // Act
        var act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Cannot cancel items from a cancelled sale*");
    }

    [Fact]
    public async Task Handle_WithAlreadyCancelledItem_ShouldThrowException()
    {
        // Arrange
        var existingSale = _saleFaker.GenerateWithItems();
        var itemToCancel = existingSale.Items.First();
        itemToCancel.Cancel();
        SetupSaleRepository(existingSale);

        var command = new CancelItemCommand(existingSale.Id, itemToCancel.Id);
        var handler = new CancelItemHandler(SaleRepository, Mapper, EventPublisher);

        // Act
        var act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Item is already cancelled*");
    }
}
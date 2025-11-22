using Ambev.DeveloperEvaluation.Application.Sales.CancelSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Unit.Application.TestData;
using Ambev.DeveloperEvaluation.Unit.TestHelpers;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class CancelSaleHandlerTests : TestBase
{
    private readonly SaleFaker _saleFaker = new();

    [Fact]
    public async Task Handle_WithValidSale_ShouldCancelAndPublishEvent()
    {
        // Arrange
        var existingSale = _saleFaker.GenerateWithItems();
        SetupSaleRepository(existingSale);
        SaleRepository.SetupUpdateAsync();

        var command = new CancelSaleCommand(existingSale.Id);
        var handler = new CancelSaleHandler(SaleRepository, Mapper, EventPublisher);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsCancelled.Should().BeTrue();
        result.CancelledAt.Should().NotBeNull();

        await SaleRepository.Received(1).UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
        await EventPublisher.Received(1).PublishAsync(
            Arg.Any<SaleCancelledEvent>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithNonExistentSale_ShouldThrowException()
    {
        // Arrange
        SetupSaleRepositoryNotFound();
        var saleId = Guid.NewGuid();
        var command = new CancelSaleCommand(saleId);
        var handler = new CancelSaleHandler(SaleRepository, Mapper, EventPublisher);

        // Act
        var act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"*Sale with ID {saleId} not found*");
    }

    [Fact]
    public async Task Handle_WithAlreadyCancelledSale_ShouldThrowException()
    {
        // Arrange
        var cancelledSale = _saleFaker.GenerateCancelled();
        SetupSaleRepository(cancelledSale);

        var command = new CancelSaleCommand(cancelledSale.Id);
        var handler = new CancelSaleHandler(SaleRepository, Mapper, EventPublisher);

        // Act
        var act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Sale is already cancelled*");
    }
}

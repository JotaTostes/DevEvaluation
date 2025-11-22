using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;
using Ambev.DeveloperEvaluation.Unit.Application.TestData;
using Ambev.DeveloperEvaluation.Unit.TestHelpers;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

/// <summary>
/// Unit tests for DeleteSaleHandler
/// </summary>
public class DeleteSaleHandlerTests : TestBase
{
    private readonly SaleFaker _saleFaker = new();

    [Fact]
    public async Task Handle_WithExistingSale_ShouldDeleteAndReturnTrue()
    {
        // Arrange
        var existingSale = _saleFaker.GenerateWithItems();
        SetupSaleRepository(existingSale);

        var command = new DeleteSaleCommand(existingSale.Id);
        var handler = new DeleteSaleHandler(SaleRepository);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        await SaleRepository.Received(1).DeleteAsync(existingSale.Id, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithNonExistentSale_ShouldThrowException()
    {
        // Arrange
        SetupSaleRepositoryNotFound();
        var saleId = Guid.NewGuid();
        var command = new DeleteSaleCommand(saleId);
        var handler = new DeleteSaleHandler(SaleRepository);

        // Act
        var act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"*Sale with ID {saleId} not found*");
    }
}
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Unit.Application.TestData;
using Ambev.DeveloperEvaluation.Unit.TestHelpers;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;


public class GetSaleHandlerTests : TestBase
{
    private readonly SaleFaker _saleFaker = new();

    [Fact]
    public async Task Handle_WithExistingSale_ShouldReturnSaleDto()
    {
        // Arrange
        var existingSale = _saleFaker.GenerateWithItems(3);
        SetupSaleRepository(existingSale);

        var query = new GetSaleQuery(existingSale.Id);
        var handler = new GetSaleHandler(SaleRepository, Mapper);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(existingSale.Id);
        result.SaleNumber.Should().Be(existingSale.SaleNumber);
        result.CustomerName.Should().Be(existingSale.CustomerName);
        result.Items.Should().HaveCount(3);
    }

    [Fact]
    public async Task Handle_WithNonExistentSale_ShouldThrowException()
    {
        // Arrange
        SetupSaleRepositoryNotFound();
        var saleId = Guid.NewGuid();
        var query = new GetSaleQuery(saleId);
        var handler = new GetSaleHandler(SaleRepository, Mapper);

        // Act
        var act = async () => await handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"*Sale with ID {saleId} not found*");
    }

    [Fact]
    public async Task Handle_ShouldMapAllFieldsCorrectly()
    {
        // Arrange
        var existingSale = _saleFaker.GenerateWithItems(2);
        SetupSaleRepository(existingSale);

        var query = new GetSaleQuery(existingSale.Id);
        var handler = new GetSaleHandler(SaleRepository, Mapper);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Id.Should().Be(existingSale.Id);
        result.SaleNumber.Should().Be(existingSale.SaleNumber);
        result.SaleDate.Should().Be(existingSale.SaleDate);
        result.CustomerId.Should().Be(existingSale.CustomerId);
        result.CustomerName.Should().Be(existingSale.CustomerName);
        result.BranchId.Should().Be(existingSale.BranchId);
        result.BranchName.Should().Be(existingSale.BranchName);
        result.TotalAmount.Should().Be(existingSale.TotalAmount);
        result.Status.Should().Be((int)existingSale.Status);
        result.IsCancelled.Should().Be(existingSale.IsCancelled());
    }
}
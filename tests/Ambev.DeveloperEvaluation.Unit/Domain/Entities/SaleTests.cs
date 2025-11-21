using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Unit.Application.TestData;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

public class SaleTests
{
    private readonly SaleFaker _faker = new();
    private readonly SaleItemFaker _itemFaker = new();

    #region Initialization Tests

    [Fact]
    public void Sale_WhenCreated_ShouldHaveActiveStatus()
    {
        // Arrange & Act
        var sale = new Sale();

        // Assert
        sale.Status.Should().Be(SaleStatus.Active);
        sale.IsCancelled().Should().BeFalse();
    }

    [Fact]
    public void Sale_WhenCreated_ShouldHaveEmptyItems()
    {
        // Arrange & Act
        var sale = new Sale();

        // Assert
        sale.Items.Should().BeEmpty();
    }

    #endregion

    #region CalculateTotalAmount Tests

    [Fact]
    public void CalculateTotalAmount_ShouldSumAllItemTotals()
    {
        // Arrange
        var sale = new Sale();

        var item1 = new SaleItem { ProductId = "P1", ProductName = "Product 1", UnitPrice = 10.00m };
        item1.SetQuantity(2); // 20.00 (no discount)

        var item2 = new SaleItem { ProductId = "P2", ProductName = "Product 2", UnitPrice = 5.00m };
        item2.SetQuantity(5); // 22.50 (10% discount: 25 - 2.5)

        sale.Items.Add(item1);
        sale.Items.Add(item2);

        // Act
        sale.CalculateTotalAmount();

        // Assert
        sale.TotalAmount.Should().Be(42.50m);
    }

    [Fact]
    public void CalculateTotalAmount_WithNoItems_ShouldBeZero()
    {
        // Arrange
        var sale = new Sale();

        // Act
        sale.CalculateTotalAmount();

        // Assert
        sale.TotalAmount.Should().Be(0);
    }

    #endregion

    #region Cancel Tests

    [Fact]
    public void Cancel_ShouldSetStatusToCancelled()
    {
        // Arrange
        var sale = _faker.GenerateWithItems();

        // Act
        sale.Cancel();

        // Assert
        sale.Status.Should().Be(SaleStatus.Cancelled);
        sale.IsCancelled().Should().BeTrue();
        sale.CancelledAt.Should().NotBeNull();
        sale.CancelledAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Cancel_WhenAlreadyCancelled_ShouldThrowException()
    {
        // Arrange
        var sale = _faker.GenerateCancelled();

        // Act
        var act = () => sale.Cancel();

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Sale is already cancelled*");
    }

    #endregion

    #region AddItem Tests

    [Fact]
    public void AddItem_ShouldAddItemAndRecalculateTotal()
    {
        // Arrange
        var sale = new Sale();
        var item = new SaleItem { ProductId = "P1", ProductName = "Product", UnitPrice = 10.00m };
        item.SetQuantity(3);

        // Act
        sale.AddItem(item);

        // Assert
        sale.Items.Should().HaveCount(1);
        sale.TotalAmount.Should().Be(30.00m);
    }

    [Fact]
    public void AddItem_WhenSaleIsCancelled_ShouldThrowException()
    {
        // Arrange
        var sale = _faker.GenerateCancelled();
        var item = _itemFaker.Generate();

        // Act
        var act = () => sale.AddItem(item);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Cannot add items to a cancelled sale*");
    }

    #endregion

    #region RemoveItem Tests

    [Fact]
    public void RemoveItem_ShouldRemoveItemAndRecalculateTotal()
    {
        // Arrange
        var sale = new Sale();
        var item1 = new SaleItem { ProductId = "P1", ProductName = "Product 1", UnitPrice = 10.00m };
        item1.SetQuantity(2);
        var item2 = new SaleItem { ProductId = "P2", ProductName = "Product 2", UnitPrice = 5.00m };
        item2.SetQuantity(2);

        sale.AddItem(item1);
        sale.AddItem(item2);

        // Act
        sale.RemoveItem(item1);

        // Assert
        sale.Items.Should().HaveCount(1);
        sale.TotalAmount.Should().Be(10.00m);
    }

    [Fact]
    public void RemoveItem_WhenSaleIsCancelled_ShouldThrowException()
    {
        // Arrange
        var sale = _faker.GenerateWithItems();
        var item = sale.Items.First();
        sale.Cancel();

        // Act
        var act = () => sale.RemoveItem(item);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Cannot remove items from a cancelled sale*");
    }

    #endregion

    #region IsCancelled Tests

    [Fact]
    public void IsCancelled_WhenActive_ShouldReturnFalse()
    {
        // Arrange
        var sale = _faker.GenerateWithItems();

        // Act & Assert
        sale.IsCancelled().Should().BeFalse();
    }

    [Fact]
    public void IsCancelled_WhenCancelled_ShouldReturnTrue()
    {
        // Arrange
        var sale = _faker.GenerateCancelled();

        // Act & Assert
        sale.IsCancelled().Should().BeTrue();
    }

    #endregion
}

using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Unit.Application.TestData;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

public class SaleItemTests
{
    private readonly SaleItemFaker _faker = new();

    #region Quantity and Discount Tests

    [Theory]
    [InlineData(1, 0)]
    [InlineData(2, 0)]
    [InlineData(3, 0)]
    public void SetQuantity_WhenLessThan4_ShouldHaveNoDiscount(int quantity, decimal expectedDiscount)
    {
        // Arrange
        var item = _faker.Generate();

        // Act
        item.SetQuantity(quantity);

        // Assert
        item.Quantity.Should().Be(quantity);
        item.Discount.Should().Be(expectedDiscount);
    }

    [Theory]
    [InlineData(4, 10)]
    [InlineData(5, 10)]
    [InlineData(9, 10)]
    public void SetQuantity_WhenBetween4And9_ShouldHave10PercentDiscount(int quantity, decimal expectedDiscount)
    {
        // Arrange
        var item = _faker.Generate();

        // Act
        item.SetQuantity(quantity);

        // Assert
        item.Quantity.Should().Be(quantity);
        item.Discount.Should().Be(expectedDiscount);
    }

    [Theory]
    [InlineData(10, 20)]
    [InlineData(15, 20)]
    [InlineData(20, 20)]
    public void SetQuantity_WhenBetween10And20_ShouldHave20PercentDiscount(int quantity, decimal expectedDiscount)
    {
        // Arrange
        var item = _faker.Generate();

        // Act
        item.SetQuantity(quantity);

        // Assert
        item.Quantity.Should().Be(quantity);
        item.Discount.Should().Be(expectedDiscount);
    }

    [Fact]
    public void SetQuantity_WhenGreaterThan20_ShouldThrowException()
    {
        // Arrange
        var item = _faker.Generate();

        // Act
        var act = () => item.SetQuantity(21);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Cannot sell more than 20 identical items*");
    }

    [Fact]
    public void SetQuantity_WhenZeroOrNegative_ShouldThrowException()
    {
        // Arrange
        var item = _faker.Generate();

        // Act
        var actZero = () => item.SetQuantity(0);
        var actNegative = () => item.SetQuantity(-1);

        // Assert
        actZero.Should().Throw<ArgumentException>()
            .WithMessage("*Quantity must be greater than zero*");
        actNegative.Should().Throw<ArgumentException>()
            .WithMessage("*Quantity must be greater than zero*");
    }

    #endregion

    #region Total Amount Calculation Tests

    [Fact]
    public void SetQuantity_ShouldCalculateTotalAmountCorrectly_WithNoDiscount()
    {
        // Arrange
        var item = new SaleItem
        {
            ProductId = "PROD-001",
            ProductName = "Test Product",
            UnitPrice = 10.00m
        };

        // Act
        item.SetQuantity(3); // No discount

        // Assert
        item.TotalAmount.Should().Be(30.00m); // 3 * 10 = 30, no discount
    }

    [Fact]
    public void SetQuantity_ShouldCalculateTotalAmountCorrectly_With10PercentDiscount()
    {
        // Arrange
        var item = new SaleItem
        {
            ProductId = "PROD-001",
            ProductName = "Test Product",
            UnitPrice = 10.00m
        };

        // Act
        item.SetQuantity(5); // 10% discount

        // Assert
        item.TotalAmount.Should().Be(45.00m); // 5 * 10 = 50, 10% off = 45
    }

    [Fact]
    public void SetQuantity_ShouldCalculateTotalAmountCorrectly_With20PercentDiscount()
    {
        // Arrange
        var item = new SaleItem
        {
            ProductId = "PROD-001",
            ProductName = "Test Product",
            UnitPrice = 10.00m
        };

        // Act
        item.SetQuantity(10); // 20% discount

        // Assert
        item.TotalAmount.Should().Be(80.00m); // 10 * 10 = 100, 20% off = 80
    }

    #endregion

    #region Cancel Tests

    [Fact]
    public void Cancel_ShouldSetIsCancelledToTrue()
    {
        // Arrange
        var item = _faker.Generate();

        // Act
        item.Cancel();

        // Assert
        item.IsCancelled.Should().BeTrue();
        item.CancelledAt.Should().NotBeNull();
        item.CancelledAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Cancel_WhenAlreadyCancelled_ShouldThrowException()
    {
        // Arrange
        var item = _faker.Generate();
        item.Cancel();

        // Act
        var act = () => item.Cancel();

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Item is already cancelled*");
    }

    #endregion

    #region UpdateUnitPrice Tests

    [Fact]
    public void UpdateUnitPrice_ShouldRecalculateTotalAmount()
    {
        // Arrange
        var item = new SaleItem
        {
            ProductId = "PROD-001",
            ProductName = "Test Product",
            UnitPrice = 10.00m
        };
        item.SetQuantity(5); // 10% discount, total = 45

        // Act
        item.UpdateUnitPrice(20.00m);

        // Assert
        item.UnitPrice.Should().Be(20.00m);
        item.TotalAmount.Should().Be(90.00m); // 5 * 20 = 100, 10% off = 90
    }

    [Fact]
    public void UpdateUnitPrice_WhenZeroOrNegative_ShouldThrowException()
    {
        // Arrange
        var item = _faker.Generate();

        // Act
        var actZero = () => item.UpdateUnitPrice(0);
        var actNegative = () => item.UpdateUnitPrice(-10);

        // Assert
        actZero.Should().Throw<ArgumentException>()
            .WithMessage("*Unit price must be greater than zero*");
        actNegative.Should().Throw<ArgumentException>()
            .WithMessage("*Unit price must be greater than zero*");
    }

    #endregion
}

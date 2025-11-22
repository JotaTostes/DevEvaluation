using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Unit.Application.TestData;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.Validators;

public class CreateSaleValidatorTests
{
    private readonly CreateSaleValidator _validator = new();
    private readonly CreateSaleCommandFaker _faker = new();

    [Fact]
    public async Task Validate_WithValidCommand_ShouldBeValid()
    {
        // Arrange
        var command = _faker.Generate();

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task Validate_WithEmptySaleNumber_ShouldBeInvalid()
    {
        // Arrange
        var command = _faker.Generate();
        command.SaleNumber = string.Empty;

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "SaleNumber");
    }

    [Fact]
    public async Task Validate_WithFutureSaleDate_ShouldBeInvalid()
    {
        // Arrange
        var command = _faker.Generate();
        command.SaleDate = DateTime.UtcNow.AddDays(1);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "SaleDate");
    }

    [Fact]
    public async Task Validate_WithEmptyCustomerId_ShouldBeInvalid()
    {
        // Arrange
        var command = _faker.Generate();
        command.CustomerId = string.Empty;

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "CustomerId");
    }

    [Fact]
    public async Task Validate_WithEmptyItems_ShouldBeInvalid()
    {
        // Arrange
        var command = _faker.Generate();
        command.Items = new List<CreateSaleItemCommand>();

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Items");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Validate_WithInvalidQuantity_ShouldBeInvalid(int quantity)
    {
        // Arrange
        var command = _faker.Generate();
        command.Items = new List<CreateSaleItemCommand>
        {
            new() { ProductId = "P1", ProductName = "Product", Quantity = quantity, UnitPrice = 10 }
        };

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName.Contains("Quantity"));
    }

    [Fact]
    public async Task Validate_WithQuantityOver20_ShouldBeInvalid()
    {
        // Arrange
        var command = _faker.Generate();
        command.Items = new List<CreateSaleItemCommand>
        {
            new() { ProductId = "P1", ProductName = "Product", Quantity = 21, UnitPrice = 10 }
        };

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName.Contains("Quantity") &&
            e.ErrorMessage.Contains("20"));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public async Task Validate_WithInvalidUnitPrice_ShouldBeInvalid(decimal unitPrice)
    {
        // Arrange
        var command = _faker.Generate();
        command.Items = new List<CreateSaleItemCommand>
        {
            new() { ProductId = "P1", ProductName = "Product", Quantity = 5, UnitPrice = unitPrice }
        };

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName.Contains("UnitPrice"));
    }

    [Fact]
    public async Task Validate_WithMultipleErrors_ShouldReturnAllErrors()
    {
        // Arrange
        var command = new CreateSaleCommand
        {
            SaleNumber = string.Empty,
            SaleDate = DateTime.UtcNow.AddDays(1),
            CustomerId = string.Empty,
            CustomerName = string.Empty,
            BranchId = string.Empty,
            BranchName = string.Empty,
            Items = new List<CreateSaleItemCommand>()
        };

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCountGreaterThan(5);
    }
}
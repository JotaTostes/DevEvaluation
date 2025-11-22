using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Services;
using Ambev.DeveloperEvaluation.ORM.Repositories;
using Ambev.DeveloperEvaluation.Unit.Application.TestData;
using Ambev.DeveloperEvaluation.Unit.TestHelpers;
using AutoMapper;
using FluentAssertions;
using FluentValidation;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class CreateSaleHandlerTests : TestBase
{
    private readonly CreateSaleCommandFaker _commandFaker = new();

    [Fact]
    public async Task Handle_WithValidCommand_ShouldCreateSaleAndPublishEvent()
    {
        // Arrange
        var command = _commandFaker.Generate();
        SaleRepository.GetBySaleNumberAsync(command.SaleNumber, Arg.Any<CancellationToken>())
            .Returns((Sale?)null);
        SaleRepository.SetupCreateAsync();

        var handler = new CreateSaleHandler(SaleRepository, Mapper, EventPublisher);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.SaleNumber.Should().Be(command.SaleNumber);
        result.CustomerName.Should().Be(command.CustomerName);
        result.Items.Should().HaveCount(command.Items.Count);

        await SaleRepository.Received(1).CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
        await EventPublisher.Received(1).PublishAsync(
            Arg.Any<SaleCreatedEvent>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithDuplicateSaleNumber_ShouldThrowException()
    {
        // Arrange
        var existingSale = new SaleFaker().GenerateWithItems();
        var command = _commandFaker.Generate();
        command.SaleNumber = existingSale.SaleNumber;

        SaleRepository.GetBySaleNumberAsync(command.SaleNumber, Arg.Any<CancellationToken>())
            .Returns(existingSale);

        var handler = new CreateSaleHandler(SaleRepository, Mapper, EventPublisher);

        // Act
        var act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"*Sale number {command.SaleNumber} already exists*");

        await SaleRepository.DidNotReceive().CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithInvalidCommand_ShouldThrowValidationException()
    {
        // Arrange
        var command = _commandFaker.GenerateInvalid();
        var handler = new CreateSaleHandler(SaleRepository, Mapper, EventPublisher);

        // Act
        var act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
        await SaleRepository.DidNotReceive().CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldCalculateDiscountsCorrectly()
    {
        // Arrange
        var command = new CreateSaleCommand
        {
            SaleNumber = "SALE-TEST",
            SaleDate = DateTime.UtcNow,
            CustomerId = "CUST-001",
            CustomerName = "Test Customer",
            BranchId = "BRANCH-001",
            BranchName = "Test Branch",
            Items = new List<CreateSaleItemCommand>
            {
                new() { ProductId = "P1", ProductName = "Product 1", Quantity = 3, UnitPrice = 10.00m }, // 0% = 30
                new() { ProductId = "P2", ProductName = "Product 2", Quantity = 5, UnitPrice = 10.00m }, // 10% = 45
                new() { ProductId = "P3", ProductName = "Product 3", Quantity = 15, UnitPrice = 10.00m } // 20% = 120
            }
        };

        SaleRepository.GetBySaleNumberAsync(command.SaleNumber, Arg.Any<CancellationToken>())
            .Returns((Sale?)null);
        SaleRepository.SetupCreateAsync();

        var handler = new CreateSaleHandler(SaleRepository, Mapper, EventPublisher);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Items.Should().HaveCount(3);
        result.Items.First(i => i.ProductId == "P1").Discount.Should().Be(0);
        result.Items.First(i => i.ProductId == "P2").Discount.Should().Be(10);
        result.Items.First(i => i.ProductId == "P3").Discount.Should().Be(20);
        result.TotalAmount.Should().Be(195.00m); // 30 + 45 + 120
    }

    [Fact]
    public async Task Handle_WithQuantityOver20_ShouldThrowValidationException()
    {
        // Arrange
        var command = _commandFaker.GenerateWithInvalidQuantity();
        var handler = new CreateSaleHandler(SaleRepository, Mapper, EventPublisher);

        // Act
        var act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .Where(e => e.Errors.Any(err => err.ErrorMessage.Contains("20")));
    }
}
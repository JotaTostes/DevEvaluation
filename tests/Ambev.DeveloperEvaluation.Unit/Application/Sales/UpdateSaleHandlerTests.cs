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

public class UpdateSaleHandlerTests : TestBase
{
    private readonly UpdateSaleCommandFaker _commandFaker = new();
    private readonly SaleFaker _saleFaker = new();

    [Fact]
    public async Task Handle_WithValidCommand_ShouldUpdateSaleAndPublishEvent()
    {
        // Arrange
        var existingSale = _saleFaker.GenerateWithItems();
        SetupSaleRepository(existingSale);
        SaleRepository.SetupUpdateAsync();

        var command = _commandFaker.GenerateForSale(existingSale.Id);
        var handler = new UpdateSaleHandler(SaleRepository, Mapper, EventPublisher);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.CustomerName.Should().Be(command.CustomerName);
        result.BranchName.Should().Be(command.BranchName);

        await SaleRepository.Received(1).UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
        await EventPublisher.Received(1).PublishAsync(
            Arg.Any<SaleModifiedEvent>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithNonExistentSale_ShouldThrowException()
    {
        // Arrange
        SetupSaleRepositoryNotFound();
        var command = _commandFaker.Generate();
        var handler = new UpdateSaleHandler(SaleRepository, Mapper, EventPublisher);

        // Act
        var act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"*Sale with ID {command.Id} not found*");
    }

    [Fact]
    public async Task Handle_WithCancelledSale_ShouldThrowException()
    {
        // Arrange
        var cancelledSale = _saleFaker.GenerateCancelled();
        SetupSaleRepository(cancelledSale);

        var command = _commandFaker.GenerateForSale(cancelledSale.Id);
        var handler = new UpdateSaleHandler(SaleRepository, Mapper, EventPublisher);

        // Act
        var act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Cannot update a cancelled sale*");
    }

    [Fact]
    public async Task Handle_WithInvalidCommand_ShouldThrowValidationException()
    {
        // Arrange
        var existingSale = _saleFaker.GenerateWithItems();
        SetupSaleRepository(existingSale);

        var command = _commandFaker.GenerateInvalid();
        command.Id = existingSale.Id;

        var handler = new UpdateSaleHandler(SaleRepository, Mapper, EventPublisher);

        // Act
        var act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task Handle_ShouldRecalculateTotalAmount()
    {
        // Arrange
        var existingSale = _saleFaker.GenerateWithItems();
        var originalTotal = existingSale.TotalAmount;
        SetupSaleRepository(existingSale);
        SaleRepository.SetupUpdateAsync();

        var command = new UpdateSaleCommand
        {
            Id = existingSale.Id,
            CustomerId = existingSale.CustomerId,
            CustomerName = existingSale.CustomerName,
            BranchId = existingSale.BranchId,
            BranchName = existingSale.BranchName,
            Items = new List<UpdateSaleItemCommand>
            {
                new() { ProductId = "P1", ProductName = "New Product", Quantity = 10, UnitPrice = 100.00m }
            }
        };

        var handler = new UpdateSaleHandler(SaleRepository, Mapper, EventPublisher);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.TotalAmount.Should().Be(800.00m); // 10 * 100 = 1000, 20% off = 800
        result.Items.Should().HaveCount(1);
    }
}

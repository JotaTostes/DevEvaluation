using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Unit.Application.TestData;
using Ambev.DeveloperEvaluation.Unit.TestHelpers;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;


public class GetSalesHandlerTests : TestBase
{
    private readonly SaleFaker _saleFaker = new();

    [Fact]
    public async Task Handle_ShouldReturnPaginatedList()
    {
        // Arrange
        var sales = _saleFaker.GenerateMany(5);
        SetupSaleRepository(sales, 15);

        var query = new GetSalesQuery
        {
            PageNumber = 1,
            PageSize = 5
        };

        var handler = new GetSalesHandler(SaleRepository, Mapper);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(5);
        result.PageNumber.Should().Be(1);
        result.PageSize.Should().Be(5);
        result.TotalCount.Should().Be(15);
        result.TotalPages.Should().Be(3);
        result.HasPreviousPage.Should().BeFalse();
        result.HasNextPage.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WithFilters_ShouldPassFiltersToRepository()
    {
        // Arrange
        var sales = _saleFaker.GenerateMany(3);
        SetupSaleRepository(sales, 3);

        var query = new GetSalesQuery
        {
            PageNumber = 1,
            PageSize = 10,
            CustomerId = "CUST-001",
            BranchId = "BRANCH-001",
            StartDate = DateTime.UtcNow.AddDays(-30),
            EndDate = DateTime.UtcNow,
            Status = 1,
            OrderBy = "TotalAmount",
            Ascending = true
        };

        var handler = new GetSalesHandler(SaleRepository, Mapper);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(3);
    }

    [Fact]
    public async Task Handle_WithNoResults_ShouldReturnEmptyList()
    {
        // Arrange
        SetupSaleRepository(new List<Sale>(), 0);

        var query = new GetSalesQuery
        {
            PageNumber = 1,
            PageSize = 10
        };

        var handler = new GetSalesHandler(SaleRepository, Mapper);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
        result.TotalPages.Should().Be(0);
        result.HasPreviousPage.Should().BeFalse();
        result.HasNextPage.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_OnMiddlePage_ShouldHaveBothPreviousAndNextPages()
    {
        // Arrange
        var sales = _saleFaker.GenerateMany(5);
        SetupSaleRepository(sales, 25);

        var query = new GetSalesQuery
        {
            PageNumber = 2,
            PageSize = 5
        };

        var handler = new GetSalesHandler(SaleRepository, Mapper);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.PageNumber.Should().Be(2);
        result.TotalPages.Should().Be(5);
        result.HasPreviousPage.Should().BeTrue();
        result.HasNextPage.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_OnLastPage_ShouldHaveNoPreviousButNoNextPage()
    {
        // Arrange
        var sales = _saleFaker.GenerateMany(3);
        SetupSaleRepository(sales, 13);

        var query = new GetSalesQuery
        {
            PageNumber = 3,
            PageSize = 5
        };

        var handler = new GetSalesHandler(SaleRepository, Mapper);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.PageNumber.Should().Be(3);
        result.TotalPages.Should().Be(3);
        result.HasPreviousPage.Should().BeTrue();
        result.HasNextPage.Should().BeFalse();
    }
}
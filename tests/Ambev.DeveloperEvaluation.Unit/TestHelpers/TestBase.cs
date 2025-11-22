using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Services;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Unit.TestHelpers;

public abstract class TestBase
{
    /// <summary>
    /// Gets the AutoMapper instance
    /// </summary>
    protected IMapper Mapper { get; }

    /// <summary>
    /// Gets the mocked sale repository
    /// </summary>
    protected ISaleRepository SaleRepository { get; set; }

    /// <summary>
    /// Gets the mocked event publisher
    /// </summary>
    protected IEventPublisher EventPublisher { get; set; }

    /// <summary>
    /// Initializes a new instance of TestBase
    /// </summary>
    protected TestBase()
    {
        Mapper = AutoMapperTestHelper.GetMapper();
        SaleRepository = RepositoryMockHelper.CreateSaleRepositoryMock();
        EventPublisher = EventPublisherMockHelper.CreateEventPublisherMock();
    }

    /// <summary>
    /// Sets up the repository to return a specific sale
    /// </summary>
    protected void SetupSaleRepository(Sale sale)
    {
        SaleRepository = RepositoryMockHelper.CreateSaleRepositoryMock(sale);
    }

   
    protected void SetupSaleRepository(List<Sale> sales, int totalCount)
    {
        SaleRepository = RepositoryMockHelper.CreateSaleRepositoryMock(sales, totalCount);
    }

    protected void SetupSaleRepositoryNotFound()
    {
        SaleRepository = RepositoryMockHelper.CreateSaleRepositoryMockNotFound();
    }
}

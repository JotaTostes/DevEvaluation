using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using NSubstitute;

namespace Ambev.DeveloperEvaluation.Unit.TestHelpers;

public static class RepositoryMockHelper
{
    public static ISaleRepository CreateSaleRepositoryMock()
    {
        return Substitute.For<ISaleRepository>();
    }

    
    public static ISaleRepository CreateSaleRepositoryMock(Sale sale)
    {
        var repository = Substitute.For<ISaleRepository>();

        repository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>())
            .Returns(sale);

        repository.GetBySaleNumberAsync(sale.SaleNumber, Arg.Any<CancellationToken>())
            .Returns(sale);

        repository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Sale>());

        repository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Sale>());

        repository.DeleteAsync(sale.Id, Arg.Any<CancellationToken>())
            .Returns(true);

        return repository;
    }

    
    public static ISaleRepository CreateSaleRepositoryMock(List<Sale> sales, int totalCount)
    {
        var repository = Substitute.For<ISaleRepository>();

        repository.GetAllAsync(
            Arg.Any<int>(),
            Arg.Any<int>(),
            Arg.Any<string?>(),
            Arg.Any<string?>(),
            Arg.Any<DateTime?>(),
            Arg.Any<DateTime?>(),
            Arg.Any<int?>(),
            Arg.Any<string>(),
            Arg.Any<bool>(),
            Arg.Any<CancellationToken>())
            .Returns((sales.AsEnumerable(), totalCount));

        return repository;
    }

   
    public static ISaleRepository CreateSaleRepositoryMockNotFound()
    {
        var repository = Substitute.For<ISaleRepository>();

        repository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((Sale?)null);

        repository.GetBySaleNumberAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns((Sale?)null);

        repository.DeleteAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(false);

        return repository;
    }

    
    public static void SetupCreateAsync(this ISaleRepository repository)
    {
        repository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Sale>());
    }

    public static void SetupUpdateAsync(this ISaleRepository repository)
    {
        repository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Sale>());
    }
}

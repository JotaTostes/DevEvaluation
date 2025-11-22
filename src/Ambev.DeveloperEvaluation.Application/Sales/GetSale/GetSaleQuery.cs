using Ambev.DeveloperEvaluation.Application.Sales.DTOs;
using MediatR;


namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale;

public class GetSaleQuery : IRequest<SaleDto>
{
    /// <summary>
    /// Gets or sets the sale identifier
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Initializes a new instance of GetSaleQuery
    /// </summary>
    /// <param name="id">The sale identifier</param>
    public GetSaleQuery(Guid id)
    {
        Id = id;
    }
}

using Ambev.DeveloperEvaluation.Application.Sales.DTOs;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale;

public class CancelSaleCommand : IRequest<SaleDto>
{
    /// <summary>
    /// Gets or sets the sale identifier to cancel
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Initializes a new instance of CancelSaleCommand
    /// </summary>
    /// <param name="id">The sale identifier</param>
    public CancelSaleCommand(Guid id)
    {
        Id = id;
    }
}

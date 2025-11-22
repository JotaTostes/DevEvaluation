using Ambev.DeveloperEvaluation.Application.Sales.DTOs;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
public class UpdateSaleCommand : IRequest<SaleDto>
{
    /// <summary>
    /// Gets or sets the sale identifier
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the customer identifier
    /// </summary>
    public string CustomerId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the customer name
    /// </summary>
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the branch identifier
    /// </summary>
    public string BranchId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the branch name
    /// </summary>
    public string BranchName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the sale items to add or update
    /// </summary>
    public List<UpdateSaleItemCommand> Items { get; set; } = new();
}

/// <summary>
/// Command for updating a sale item
/// </summary>
public class UpdateSaleItemCommand
{
    /// <summary>
    /// Gets or sets the item identifier (null for new items)
    /// </summary>
    public Guid? Id { get; set; }

    /// <summary>
    /// Gets or sets the product identifier
    /// </summary>
    public string ProductId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the product name
    /// </summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the quantity
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Gets or sets the unit price
    /// </summary>
    public decimal UnitPrice { get; set; }
}
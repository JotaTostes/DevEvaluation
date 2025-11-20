using Ambev.DeveloperEvaluation.Application.Sales.DTOs;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale;

public class GetSalesQuery : IRequest<PaginatedList<SaleDto>>
{
    /// <summary>
    /// Gets or sets the page number (default: 1)
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Gets or sets the page size (default: 10)
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// Gets or sets the customer ID filter
    /// </summary>
    public string? CustomerId { get; set; }

    /// <summary>
    /// Gets or sets the branch ID filter
    /// </summary>
    public string? BranchId { get; set; }

    /// <summary>
    /// Gets or sets the start date filter
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// Gets or sets the end date filter
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Gets or sets the status filter (1 = Active, 2 = Cancelled)
    /// </summary>
    public int? Status { get; set; }

    /// <summary>
    /// Gets or sets the property to order by (default: SaleDate)
    /// </summary>
    public string OrderBy { get; set; } = "SaleDate";

    /// <summary>
    /// Gets or sets the sort direction (default: false - descending)
    /// </summary>
    public bool Ascending { get; set; } = false;
}

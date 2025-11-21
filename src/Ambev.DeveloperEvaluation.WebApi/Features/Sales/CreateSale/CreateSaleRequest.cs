using System.ComponentModel.DataAnnotations;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;

public class CreateSaleRequest
{
    /// <summary>
    /// Gets or sets the sale number
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string SaleNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the sale date
    /// </summary>
    [Required]
    public DateTime SaleDate { get; set; }

    /// <summary>
    /// Gets or sets the customer identifier
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string CustomerId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the customer name
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the branch identifier
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string BranchId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the branch name
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string BranchName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the sale items
    /// </summary>
    [Required]
    [MinLength(1)]
    public List<CreateSaleItemRequest> Items { get; set; } = new();
}

/// <summary>
/// Request model for creating a sale item
/// </summary>
public class CreateSaleItemRequest
{
    /// <summary>
    /// Gets or sets the product identifier
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string ProductId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the product name
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the quantity (1-20)
    /// </summary>
    [Required]
    [Range(1, 20, ErrorMessage = "Quantity must be between 1 and 20")]
    public int Quantity { get; set; }

    /// <summary>
    /// Gets or sets the unit price
    /// </summary>
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Unit price must be greater than zero")]
    public decimal UnitPrice { get; set; }
}

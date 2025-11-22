using System.ComponentModel.DataAnnotations;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;

public class UpdateSaleRequest
{
    
    [Required]
    [MaxLength(100)]
    public string CustomerId { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string CustomerName { get; set; } = string.Empty;

 
    [Required]
    [MaxLength(100)]
    public string BranchId { get; set; } = string.Empty;

  
    [Required]
    [MaxLength(200)]
    public string BranchName { get; set; } = string.Empty;


    [Required]
    [MinLength(1)]
    public List<UpdateSaleItemRequest> Items { get; set; } = new();
}


public class UpdateSaleItemRequest
{
    public Guid? Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string ProductId { get; set; } = string.Empty;

    
    [Required]
    [MaxLength(200)]
    public string ProductName { get; set; } = string.Empty;

    [Required]
    [Range(1, 20, ErrorMessage = "Quantity must be between 1 and 20")]
    public int Quantity { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Unit price must be greater than zero")]
    public decimal UnitPrice { get; set; }
}

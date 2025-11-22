using Ambev.DeveloperEvaluation.Application.Sales.DTOs;
using MediatR;


namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

public class CreateSaleCommand : IRequest<SaleDto>
{

    public string SaleNumber { get; set; } = string.Empty;


    public DateTime SaleDate { get; set; }


    public string CustomerId { get; set; } = string.Empty;


    public string CustomerName { get; set; } = string.Empty;


    public string BranchId { get; set; } = string.Empty;


    public string BranchName { get; set; } = string.Empty;


    public List<CreateSaleItemCommand> Items { get; set; } = new();
}


public class CreateSaleItemCommand
{

    public string ProductId { get; set; } = string.Empty;


    public string ProductName { get; set; } = string.Empty;

    public int Quantity { get; set; }


    public decimal UnitPrice { get; set; }
}

using Ambev.DeveloperEvaluation.Application.Sales.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelItem;

public class CancelItemCommand : IRequest<SaleDto>
{
    public Guid SaleId { get; set; }
    public Guid ItemId { get; set; }
    public CancelItemCommand(Guid saleId, Guid itemId)
    {
        SaleId = saleId;
        ItemId = itemId;
    }
}

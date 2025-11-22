using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Domain.Events;

public class SaleCancelledEvent
{

    public Guid SaleId { get; set; }


    public string SaleNumber { get; set; } = string.Empty;


    public string CustomerId { get; set; } = string.Empty;

    public string CustomerName { get; set; } = string.Empty;

    public string BranchId { get; set; } = string.Empty;

    public decimal TotalAmount { get; set; }

    public DateTime CancelledAt { get; set; }

    public DateTime OccurredAt { get; set; }
    public SaleCancelledEvent()
    {
        OccurredAt = DateTime.UtcNow;
    }
}

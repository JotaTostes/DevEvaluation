using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Domain.Events;

public class SaleModifiedEvent
{
    public Guid SaleId { get; set; }
    public string SaleNumber { get; set; } = string.Empty;
    public string CustomerId { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string BranchId { get; set; } = string.Empty;
    public string BranchName { get; set; } = string.Empty;
    public decimal PreviousTotalAmount { get; set; }
    public decimal NewTotalAmount { get; set; }
    public DateTime OccurredAt { get; set; }
    public SaleModifiedEvent()
    {
        OccurredAt = DateTime.UtcNow;
    }
}

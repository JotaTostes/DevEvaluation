using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Domain.Events;

public class ItemCancelledEvent
{
    public Guid ItemId { get; set; }

    public Guid SaleId { get; set; }

    public string SaleNumber { get; set; } = string.Empty;

    public string ProductId { get; set; } = string.Empty;

    public string ProductName { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public decimal TotalAmount { get; set; }

    public DateTime CancelledAt { get; set; }

    public DateTime OccurredAt { get; set; }

    public ItemCancelledEvent()
    {
        OccurredAt = DateTime.UtcNow;
    }
}

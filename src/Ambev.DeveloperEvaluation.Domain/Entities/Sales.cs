using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class Sale : BaseEntity
{

    public string SaleNumber { get; set; } = string.Empty;
    public DateTime SaleDate { get; set; }
    public string CustomerId { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string BranchId { get; set; } = string.Empty;
    public string BranchName { get; set; } = string.Empty;
    public decimal TotalAmount { get; private set; }
    public SaleStatus Status { get; set; }
    public ICollection<SaleItem> Items { get; set; } = new List<SaleItem>();
    public DateTime? CancelledAt { get; set; }

    public Sale()
    {
        Status = SaleStatus.Active;
        SaleDate = DateTime.UtcNow;
    }

    public void CalculateTotalAmount()
    {
        TotalAmount = Items.Sum(item => item.TotalAmount);
    }

    public void Cancel()
    {
        if (Status == SaleStatus.Cancelled)
            throw new InvalidOperationException("Sale is already cancelled");

        Status = SaleStatus.Cancelled;
        CancelledAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsCancelled() => Status == SaleStatus.Cancelled;

    public void AddItem(SaleItem item)
    {
        if (IsCancelled())
            throw new InvalidOperationException("Cannot add items to a cancelled sale");

        Items.Add(item);
        CalculateTotalAmount();
    }

    public void RemoveItem(SaleItem item)
    {
        if (IsCancelled())
            throw new InvalidOperationException("Cannot remove items from a cancelled sale");

        Items.Remove(item);
        CalculateTotalAmount();
    }
}
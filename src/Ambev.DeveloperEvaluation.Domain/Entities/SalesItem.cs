using Ambev.DeveloperEvaluation.Domain.Common;

namespace Ambev.DeveloperEvaluation.Domain.Entities;


public class SaleItem : BaseEntity
{
    public Guid SaleId { get; set; }
    public Sale Sale { get; set; } = null!;
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; set; }
    public decimal Discount { get; private set; }
    public decimal TotalAmount { get; private set; }
    public bool IsCancelled { get; set; }
    public DateTime? CancelledAt { get; set; }
    public void SetQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));

        if (quantity > 20)
            throw new ArgumentException("Cannot sell more than 20 identical items", nameof(quantity));

        Quantity = quantity;
        CalculateDiscount();
        CalculateTotalAmount();
    }


    private void CalculateDiscount()
    {
        if (Quantity < 4)
        {
            Discount = 0;
        }
        else if (Quantity >= 4 && Quantity < 10)
        {
            Discount = 10;
        }
        else if (Quantity >= 10 && Quantity <= 20)
        {
            Discount = 20;
        }
    }

    private void CalculateTotalAmount()
    {
        var subtotal = Quantity * UnitPrice;
        var discountAmount = subtotal * (Discount / 100);
        TotalAmount = subtotal - discountAmount;
    }


    public void Cancel()
    {
        if (IsCancelled)
            throw new InvalidOperationException("Item is already cancelled");

        IsCancelled = true;
        CancelledAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateUnitPrice(decimal unitPrice)
    {
        if (unitPrice <= 0)
            throw new ArgumentException("Unit price must be greater than zero", nameof(unitPrice));

        UnitPrice = unitPrice;
        CalculateTotalAmount();
    }
}
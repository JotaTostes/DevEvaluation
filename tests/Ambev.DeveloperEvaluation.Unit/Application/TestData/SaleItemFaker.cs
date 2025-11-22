using Ambev.DeveloperEvaluation.Domain.Entities;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.TestData;

public sealed class SaleItemFaker : Faker<SaleItem>
{
    public SaleItemFaker()
    {
        RuleFor(i => i.Id, f => Guid.NewGuid());
        RuleFor(i => i.ProductId, f => f.Random.Replace("PROD-####"));
        RuleFor(i => i.ProductName, f => f.Commerce.ProductName());
        RuleFor(i => i.UnitPrice, f => f.Finance.Amount(1, 100));
        RuleFor(i => i.CreatedAt, f => f.Date.Past(1));

        CustomInstantiator(f =>
        {
            var item = new SaleItem
            {
                Id = Guid.NewGuid(),
                ProductId = f.Random.Replace("PROD-####"),
                ProductName = f.Commerce.ProductName(),
                UnitPrice = f.Finance.Amount(1, 100),
                CreatedAt = f.Date.Past(1)
            };
            item.SetQuantity(f.Random.Int(1, 20));
            return item;
        });
    }

    /// <summary>
    /// Generates a SaleItem with no discount (quantity 1-3)
    /// </summary>
    public SaleItem GenerateNoDiscount()
    {
        var item = Generate();
        item.SetQuantity(new Faker().Random.Int(1, 3));
        return item;
    }

    /// <summary>
    /// Generates a SaleItem with 10% discount (quantity 4-9)
    /// </summary>
    public SaleItem Generate10PercentDiscount()
    {
        var item = Generate();
        item.SetQuantity(new Faker().Random.Int(4, 9));
        return item;
    }

    /// <summary>
    /// Generates a SaleItem with 20% discount (quantity 10-20)
    /// </summary>
    public SaleItem Generate20PercentDiscount()
    {
        var item = Generate();
        item.SetQuantity(new Faker().Random.Int(10, 20));
        return item;
    }

    /// <summary>
    /// Generates a SaleItem with specific quantity
    /// </summary>
    public SaleItem GenerateWithQuantity(int quantity)
    {
        var item = Generate();
        item.SetQuantity(quantity);
        return item;
    }
}

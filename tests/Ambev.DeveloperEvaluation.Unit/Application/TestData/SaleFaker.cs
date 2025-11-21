using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.TestData;

public sealed class SaleFaker : Faker<Sale>
{
    private readonly SaleItemFaker _itemFaker = new();

    /// <summary>
    /// Initializes a new instance of SaleFaker with default rules
    /// </summary>
    public SaleFaker()
    {
        RuleFor(s => s.Id, f => Guid.NewGuid());
        RuleFor(s => s.SaleNumber, f => f.Random.Replace("SALE-######"));
        RuleFor(s => s.SaleDate, f => f.Date.Past(1));
        RuleFor(s => s.CustomerId, f => f.Random.Replace("CUST-####"));
        RuleFor(s => s.CustomerName, f => f.Person.FullName);
        RuleFor(s => s.BranchId, f => f.Random.Replace("BRANCH-###"));
        RuleFor(s => s.BranchName, f => f.Company.CompanyName());
        RuleFor(s => s.Status, f => SaleStatus.Active);
        RuleFor(s => s.CreatedAt, f => f.Date.Past(1));
    }

    /// <summary>
    /// Generates a Sale with random items
    /// </summary>
    /// <param name="itemCount">Number of items to generate (default: random 1-5)</param>
    public Sale GenerateWithItems(int? itemCount = null)
    {
        var sale = Generate();
        var count = itemCount ?? new Faker().Random.Int(1, 5);

        for (int i = 0; i < count; i++)
        {
            var item = _itemFaker.Generate();
            item.SaleId = sale.Id;
            sale.Items.Add(item);
        }

        sale.CalculateTotalAmount();
        return sale;
    }

    /// <summary>
    /// Generates a cancelled Sale
    /// </summary>
    public Sale GenerateCancelled()
    {
        var sale = GenerateWithItems();
        sale.Cancel();
        return sale;
    }

    /// <summary>
    /// Generates a Sale with items that have no discount
    /// </summary>
    public Sale GenerateWithNoDiscountItems(int itemCount = 2)
    {
        var sale = Generate();

        for (int i = 0; i < itemCount; i++)
        {
            var item = _itemFaker.GenerateNoDiscount();
            item.SaleId = sale.Id;
            sale.Items.Add(item);
        }

        sale.CalculateTotalAmount();
        return sale;
    }

    /// <summary>
    /// Generates a Sale with items that have 10% discount
    /// </summary>
    public Sale GenerateWith10PercentDiscountItems(int itemCount = 2)
    {
        var sale = Generate();

        for (int i = 0; i < itemCount; i++)
        {
            var item = _itemFaker.Generate10PercentDiscount();
            item.SaleId = sale.Id;
            sale.Items.Add(item);
        }

        sale.CalculateTotalAmount();
        return sale;
    }

    /// <summary>
    /// Generates a Sale with items that have 20% discount
    /// </summary>
    public Sale GenerateWith20PercentDiscountItems(int itemCount = 2)
    {
        var sale = Generate();

        for (int i = 0; i < itemCount; i++)
        {
            var item = _itemFaker.Generate20PercentDiscount();
            item.SaleId = sale.Id;
            sale.Items.Add(item);
        }

        sale.CalculateTotalAmount();
        return sale;
    }

    /// <summary>
    /// Generates a list of Sales
    /// </summary>
    public List<Sale> GenerateMany(int count)
    {
        var sales = new List<Sale>();
        for (int i = 0; i < count; i++)
        {
            sales.Add(GenerateWithItems());
        }
        return sales;
    }
}

using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.TestData;

public sealed class CreateSaleCommandFaker : Faker<CreateSaleCommand>
{
    public CreateSaleCommandFaker()
    {
        RuleFor(c => c.SaleNumber, f => f.Random.Replace("SALE-######"));
        RuleFor(c => c.SaleDate, f => f.Date.Recent(30));
        RuleFor(c => c.CustomerId, f => f.Random.Replace("CUST-####"));
        RuleFor(c => c.CustomerName, f => f.Person.FullName);
        RuleFor(c => c.BranchId, f => f.Random.Replace("BRANCH-###"));
        RuleFor(c => c.BranchName, f => f.Company.CompanyName());
        RuleFor(c => c.Items, f => new CreateSaleItemCommandFaker().Generate(f.Random.Int(1, 5)));
    }

    /// <summary>
    /// Generates a command with specific number of items
    /// </summary>
    public CreateSaleCommand GenerateWithItems(int itemCount)
    {
        var command = Generate();
        command.Items = new CreateSaleItemCommandFaker().Generate(itemCount);
        return command;
    }

    /// <summary>
    /// Generates a command with invalid data (empty sale number)
    /// </summary>
    public CreateSaleCommand GenerateInvalid()
    {
        var command = Generate();
        command.SaleNumber = string.Empty;
        command.Items = new List<CreateSaleItemCommand>();
        return command;
    }

    /// <summary>
    /// Generates a command with items exceeding max quantity
    /// </summary>
    public CreateSaleCommand GenerateWithInvalidQuantity()
    {
        var command = Generate();
        command.Items = new List<CreateSaleItemCommand>
        {
            new CreateSaleItemCommand
            {
                ProductId = "PROD-001",
                ProductName = "Test Product",
                Quantity = 25, // Invalid: > 20
                UnitPrice = 10.00m
            }
        };
        return command;
    }
}

public sealed class CreateSaleItemCommandFaker : Faker<CreateSaleItemCommand>
{
    /// <summary>
    /// Initializes a new instance with default rules
    /// </summary>
    public CreateSaleItemCommandFaker()
    {
        RuleFor(i => i.ProductId, f => f.Random.Replace("PROD-####"));
        RuleFor(i => i.ProductName, f => f.Commerce.ProductName());
        RuleFor(i => i.Quantity, f => f.Random.Int(1, 20));
        RuleFor(i => i.UnitPrice, f => f.Finance.Amount(1, 100));
    }

    /// <summary>
    /// Generates item with no discount (qty 1-3)
    /// </summary>
    public CreateSaleItemCommand GenerateNoDiscount()
    {
        return new Faker<CreateSaleItemCommand>()
            .RuleFor(i => i.ProductId, f => f.Random.Replace("PROD-####"))
            .RuleFor(i => i.ProductName, f => f.Commerce.ProductName())
            .RuleFor(i => i.Quantity, f => f.Random.Int(1, 3))
            .RuleFor(i => i.UnitPrice, f => f.Finance.Amount(1, 100))
            .Generate();
    }

    /// <summary>
    /// Generates item with 10% discount (qty 4-9)
    /// </summary>
    public CreateSaleItemCommand Generate10PercentDiscount()
    {
        return new Faker<CreateSaleItemCommand>()
            .RuleFor(i => i.ProductId, f => f.Random.Replace("PROD-####"))
            .RuleFor(i => i.ProductName, f => f.Commerce.ProductName())
            .RuleFor(i => i.Quantity, f => f.Random.Int(4, 9))
            .RuleFor(i => i.UnitPrice, f => f.Finance.Amount(1, 100))
            .Generate();
    }

    /// <summary>
    /// Generates item with 20% discount (qty 10-20)
    /// </summary>
    public CreateSaleItemCommand Generate20PercentDiscount()
    {
        return new Faker<CreateSaleItemCommand>()
            .RuleFor(i => i.ProductId, f => f.Random.Replace("PROD-####"))
            .RuleFor(i => i.ProductName, f => f.Commerce.ProductName())
            .RuleFor(i => i.Quantity, f => f.Random.Int(10, 20))
            .RuleFor(i => i.UnitPrice, f => f.Finance.Amount(1, 100))
            .Generate();
    }
}

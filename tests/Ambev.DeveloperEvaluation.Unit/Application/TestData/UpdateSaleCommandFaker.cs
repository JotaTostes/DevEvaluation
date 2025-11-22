using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Bogus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Unit.Application.TestData;


public sealed class UpdateSaleCommandFaker : Faker<UpdateSaleCommand>
{

    public UpdateSaleCommandFaker()
    {
        RuleFor(c => c.Id, f => Guid.NewGuid());
        RuleFor(c => c.CustomerId, f => f.Random.Replace("CUST-####"));
        RuleFor(c => c.CustomerName, f => f.Person.FullName);
        RuleFor(c => c.BranchId, f => f.Random.Replace("BRANCH-###"));
        RuleFor(c => c.BranchName, f => f.Company.CompanyName());
        RuleFor(c => c.Items, f => new UpdateSaleItemCommandFaker().Generate(f.Random.Int(1, 5)));
    }

    /// <summary>
    /// Generates a command for a specific sale ID
    /// </summary>
    public UpdateSaleCommand GenerateForSale(Guid saleId)
    {
        var command = Generate();
        command.Id = saleId;
        return command;
    }

    /// <summary>
    /// Generates an invalid command (empty customer)
    /// </summary>
    public UpdateSaleCommand GenerateInvalid()
    {
        var command = Generate();
        command.CustomerId = string.Empty;
        command.CustomerName = string.Empty;
        command.Items = new List<UpdateSaleItemCommand>();
        return command;
    }
}

/// <summary>
/// Faker for generating UpdateSaleItemCommand test data
/// </summary>
public sealed class UpdateSaleItemCommandFaker : Faker<UpdateSaleItemCommand>
{
    /// <summary>
    /// Initializes a new instance with default rules
    /// </summary>
    public UpdateSaleItemCommandFaker()
    {
        RuleFor(i => i.Id, f => f.Random.Bool() ? Guid.NewGuid() : null);
        RuleFor(i => i.ProductId, f => f.Random.Replace("PROD-####"));
        RuleFor(i => i.ProductName, f => f.Commerce.ProductName());
        RuleFor(i => i.Quantity, f => f.Random.Int(1, 20));
        RuleFor(i => i.UnitPrice, f => f.Finance.Amount(1, 100));
    }

    /// <summary>
    /// Generates a new item (no ID)
    /// </summary>
    public UpdateSaleItemCommand GenerateNew()
    {
        var item = Generate();
        item.Id = null;
        return item;
    }

    /// <summary>
    /// Generates an existing item (with ID)
    /// </summary>
    public UpdateSaleItemCommand GenerateExisting(Guid itemId)
    {
        var item = Generate();
        item.Id = itemId;
        return item;
    }
}
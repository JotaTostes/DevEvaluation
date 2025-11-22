using Ambev.DeveloperEvaluation.Domain.Entities;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Domain.Validation;

public class SaleItemValidator : AbstractValidator<SaleItem>
{
    /// <summary>
    /// Initializes a new instance of the SaleItemValidator with validation rules.
    /// </summary>
    public SaleItemValidator()
    {
        RuleFor(item => item.ProductId)
            .NotEmpty()
            .WithMessage("Product ID is required")
            .MaximumLength(100)
            .WithMessage("Product ID cannot exceed 100 characters");

        RuleFor(item => item.ProductName)
            .NotEmpty()
            .WithMessage("Product name is required")
            .MaximumLength(200)
            .WithMessage("Product name cannot exceed 200 characters");

        RuleFor(item => item.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than zero")
            .LessThanOrEqualTo(20)
            .WithMessage("Cannot sell more than 20 identical items");

        RuleFor(item => item.UnitPrice)
            .GreaterThan(0)
            .WithMessage("Unit price must be greater than zero");

        RuleFor(item => item)
            .Must(item => ValidateDiscount(item))
            .WithMessage("Discount does not match quantity-based business rules");
    }

    /// <summary>
    /// Validates that the discount applied matches the business rules for quantity.
    /// </summary>
    private bool ValidateDiscount(SaleItem item)
    {
        if (item.Quantity < 4 && item.Discount > 0)
            return false;

        if (item.Quantity >= 4 && item.Quantity < 10 && item.Discount != 10)
            return false;

        if (item.Quantity >= 10 && item.Quantity <= 20 && item.Discount != 20)
            return false;

        return true;
    }
}

using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

/// <summary>
/// Implementation of ISaleRepository using Entity Framework Core.
/// </summary>
public class SaleRepository : ISaleRepository
{
    private readonly DefaultContext _context;

    /// <summary>
    /// Initializes a new instance of SaleRepository.
    /// </summary>
    /// <param name="context">The database context</param>
    public SaleRepository(DefaultContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Creates a new sale in the database.
    /// </summary>
    public async Task<Sale> CreateAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        await _context.Sales.AddAsync(sale, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return sale;
    }

    /// <summary>
    /// Retrieves a sale by its unique identifier, including its items.
    /// </summary>
    public async Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    /// <summary>
    /// Retrieves a sale by its sale number, including its items.
    /// </summary>
    public async Task<Sale?> GetBySaleNumberAsync(string saleNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.SaleNumber == saleNumber, cancellationToken);
    }

    /// <summary>
    /// Retrieves all sales with filtering, pagination and sorting capabilities.
    /// </summary>
    public async Task<(IEnumerable<Sale> Sales, int TotalCount)> GetAllAsync(
        int pageNumber = 1,
        int pageSize = 10,
        string? customerId = null,
        string? branchId = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        int? status = null,
        string orderBy = "SaleDate",
        bool ascending = false,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Sales
            .Include(s => s.Items)
            .AsQueryable();

        if (!string.IsNullOrEmpty(customerId))
        {
            query = query.Where(s => s.CustomerId == customerId);
        }

        if (!string.IsNullOrEmpty(branchId))
        {
            query = query.Where(s => s.BranchId == branchId);
        }

        if (startDate.HasValue)
        {
            query = query.Where(s => s.SaleDate >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(s => s.SaleDate <= endDate.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(s => (int)s.Status == status.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        query = orderBy.ToLower() switch
        {
            "salenumber" => ascending
                ? query.OrderBy(s => s.SaleNumber)
                : query.OrderByDescending(s => s.SaleNumber),
            "customer" => ascending
                ? query.OrderBy(s => s.CustomerName)
                : query.OrderByDescending(s => s.CustomerName),
            "branch" => ascending
                ? query.OrderBy(s => s.BranchName)
                : query.OrderByDescending(s => s.BranchName),
            "totalamount" => ascending
                ? query.OrderBy(s => s.TotalAmount)
                : query.OrderByDescending(s => s.TotalAmount),
            "status" => ascending
                ? query.OrderBy(s => s.Status)
                : query.OrderByDescending(s => s.Status),
            _ => ascending
                ? query.OrderBy(s => s.SaleDate)
                : query.OrderByDescending(s => s.SaleDate)
        };

        var sales = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (sales, totalCount);
    }

    /// <summary>
    /// Updates an existing sale in the database.
    /// </summary>
    public async Task<Sale> UpdateAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        _context.Sales.Update(sale);
        await _context.SaveChangesAsync(cancellationToken);
        return sale;
    }

    /// <summary>
    /// Deletes a sale from the database.
    /// </summary>
    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var sale = await GetByIdAsync(id, cancellationToken);
        if (sale == null)
            return false;

        _context.Sales.Remove(sale);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}

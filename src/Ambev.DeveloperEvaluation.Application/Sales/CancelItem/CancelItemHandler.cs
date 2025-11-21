using Ambev.DeveloperEvaluation.Application.Sales.DTOs;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Services;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelItem;

public class CancelItemHandler : IRequestHandler<CancelItemCommand, SaleDto>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly IEventPublisher _eventPublisher;

    /// <summary>
    /// Initializes a new instance of CancelItemHandler
    /// </summary>
    /// <param name="saleRepository">The sale repository</param>
    /// <param name="mapper">The AutoMapper instance</param>
    /// <param name="eventPublisher">The event publisher</param>
    public CancelItemHandler(ISaleRepository saleRepository, IMapper mapper, IEventPublisher eventPublisher)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
        _eventPublisher = eventPublisher;
    }

    /// <summary>
    /// Handles the CancelItemCommand request
    /// </summary>
    /// <param name="request">The CancelItem command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated sale data</returns>
    public async Task<SaleDto> Handle(CancelItemCommand request, CancellationToken cancellationToken)
    {
        var sale = await _saleRepository.GetByIdAsync(request.SaleId, cancellationToken);
        if (sale == null)
            throw new InvalidOperationException($"Sale with ID {request.SaleId} not found");

        if (sale.IsCancelled())
            throw new InvalidOperationException("Cannot cancel items from a cancelled sale");

        var item = sale.Items.FirstOrDefault(i => i.Id == request.ItemId);
        if (item == null)
            throw new InvalidOperationException($"Item with ID {request.ItemId} not found in sale");

        item.Cancel();

        sale.CalculateTotalAmount();

        var updatedSale = await _saleRepository.UpdateAsync(sale, cancellationToken);

        var itemCancelledEvent = new ItemCancelledEvent
        {
            ItemId = item.Id,
            SaleId = sale.Id,
            SaleNumber = sale.SaleNumber,
            ProductId = item.ProductId,
            ProductName = item.ProductName,
            Quantity = item.Quantity,
            TotalAmount = item.TotalAmount,
            CancelledAt = item.CancelledAt ?? DateTime.UtcNow
        };

        await _eventPublisher.PublishAsync(itemCancelledEvent, cancellationToken);

        return _mapper.Map<SaleDto>(updatedSale);
    }
}

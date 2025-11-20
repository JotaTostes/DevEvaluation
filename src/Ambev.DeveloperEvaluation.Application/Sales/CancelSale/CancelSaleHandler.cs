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

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale;

public class CancelSaleHandler : IRequestHandler<CancelSaleCommand, SaleDto>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly IEventPublisher _eventPublisher;

    /// <summary>
    /// Initializes a new instance of CancelSaleHandler
    /// </summary>
    /// <param name="saleRepository">The sale repository</param>
    /// <param name="mapper">The AutoMapper instance</param>
    /// <param name="eventPublisher">The event publisher</param>
    public CancelSaleHandler(ISaleRepository saleRepository, IMapper mapper, IEventPublisher eventPublisher)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
        _eventPublisher = eventPublisher;
    }

    /// <summary>
    /// Handles the CancelSaleCommand request
    /// </summary>
    /// <param name="request">The CancelSale command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The cancelled sale data</returns>
    public async Task<SaleDto> Handle(CancelSaleCommand request, CancellationToken cancellationToken)
    {
        var sale = await _saleRepository.GetByIdAsync(request.Id, cancellationToken);
        if (sale == null)
            throw new InvalidOperationException($"Sale with ID {request.Id} not found");

        sale.Cancel();

        var cancelledSale = await _saleRepository.UpdateAsync(sale, cancellationToken);

        var saleCancelledEvent = new SaleCancelledEvent
        {
            SaleId = cancelledSale.Id,
            SaleNumber = cancelledSale.SaleNumber,
            CustomerId = cancelledSale.CustomerId,
            CustomerName = cancelledSale.CustomerName,
            BranchId = cancelledSale.BranchId,
            TotalAmount = cancelledSale.TotalAmount,
            CancelledAt = cancelledSale.CancelledAt ?? DateTime.UtcNow
        };

        await _eventPublisher.PublishAsync(saleCancelledEvent, cancellationToken);

        return _mapper.Map<SaleDto>(cancelledSale);
    }
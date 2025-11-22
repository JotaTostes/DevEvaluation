using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.DTOs;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Services;
using AutoMapper;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

public class UpdateSaleHandler : IRequestHandler<UpdateSaleCommand, SaleDto>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly IEventPublisher _eventPublisher;

    /// <summary>
    /// Initializes a new instance of UpdateSaleHandler
    /// </summary>
    /// <param name="saleRepository">The sale repository</param>
    /// <param name="mapper">The AutoMapper instance</param>
    /// <param name="eventPublisher">The event publisher</param>
    public UpdateSaleHandler(ISaleRepository saleRepository, IMapper mapper, IEventPublisher eventPublisher)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
        _eventPublisher = eventPublisher;
    }

    /// <summary>
    /// Handles the UpdateSaleCommand request
    /// </summary>
    /// <param name="request">The UpdateSale command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated sale data</returns>
    public async Task<SaleDto> Handle(UpdateSaleCommand request, CancellationToken cancellationToken)
    {
        var validator = new UpdateSaleValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var existingSale = await _saleRepository.GetByIdAsync(request.Id, cancellationToken);
        if (existingSale == null)
            throw new InvalidOperationException($"Sale with ID {request.Id} not found");

        if (existingSale.IsCancelled())
            throw new InvalidOperationException("Cannot update a cancelled sale");

        existingSale.CustomerId = request.CustomerId;
        existingSale.CustomerName = request.CustomerName;
        existingSale.BranchId = request.BranchId;
        existingSale.BranchName = request.BranchName;

        existingSale.Items.Clear();

        foreach (var itemCommand in request.Items)
        {
            var item = _mapper.Map<SaleItem>(itemCommand);
            existingSale.AddItem(item);
        }

        existingSale.CalculateTotalAmount();

        var updatedSale = await _saleRepository.UpdateAsync(existingSale, cancellationToken);

        return _mapper.Map<SaleDto>(updatedSale);
    }
}

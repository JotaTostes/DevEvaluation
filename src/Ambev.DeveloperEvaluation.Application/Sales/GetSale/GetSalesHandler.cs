using Ambev.DeveloperEvaluation.Application.Sales.DTOs;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale;

public class GetSalesHandler : IRequestHandler<GetSalesQuery, PaginatedList<SaleDto>>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of GetSalesHandler
    /// </summary>
    /// <param name="saleRepository">The sale repository</param>
    /// <param name="mapper">The AutoMapper instance</param>
    public GetSalesHandler(ISaleRepository saleRepository, IMapper mapper)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Handles the GetSalesQuery request
    /// </summary>
    /// <param name="request">The GetSales query</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of sales</returns>
    public async Task<PaginatedList<SaleDto>> Handle(GetSalesQuery request, CancellationToken cancellationToken)
    {
        var (sales, totalCount) = await _saleRepository.GetAllAsync(
            pageNumber: request.PageNumber,
            pageSize: request.PageSize,
            customerId: request.CustomerId,
            branchId: request.BranchId,
            startDate: request.StartDate,
            endDate: request.EndDate,
            status: request.Status,
            orderBy: request.OrderBy,
            ascending: request.Ascending,
            cancellationToken: cancellationToken
        );

        var salesDto = _mapper.Map<List<SaleDto>>(sales);

        return new PaginatedList<SaleDto>
        {
            Data = salesDto,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }
}

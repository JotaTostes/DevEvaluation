using Ambev.DeveloperEvaluation.Application.Sales.CancelItem;
using Ambev.DeveloperEvaluation.Application.Sales.CancelSale;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;
using Ambev.DeveloperEvaluation.Application.Sales.DTOs;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Users.GetUser;
using Ambev.DeveloperEvaluation.WebApi.Middleware;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class SalesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SalesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Creates a new sale
        /// </summary>
        /// <param name="request">The sale creation request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The created sale</returns>
        /// <response code="201">Sale created successfully</response>
        /// <response code="400">Invalid request data</response>
        [HttpPost]
        [ProducesResponseType(typeof(SaleDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateSale(
            [FromBody] CreateSaleRequest request,
            CancellationToken cancellationToken)
        {
            var command = new CreateSaleCommand
            {
                SaleNumber = request.SaleNumber,
                SaleDate = request.SaleDate,
                CustomerId = request.CustomerId,
                CustomerName = request.CustomerName,
                BranchId = request.BranchId,
                BranchName = request.BranchName,
                Items = request.Items.Select(i => new CreateSaleItemCommand
                {
                    ProductId = i.ProductId,
                    ProductName = i.ProductName,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            };

            var result = await _mediator.Send(command, cancellationToken);

            return CreatedAtAction(nameof(GetSale), new { id = result.Id }, result);
        }

        /// <summary>
        /// Retrieves a sale by ID
        /// </summary>
        /// <param name="id">The sale ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The sale details</returns>
        /// <response code="200">Sale found</response>
        /// <response code="404">Sale not found</response>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(SaleDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetSale(
            [FromRoute] Guid id,
            CancellationToken cancellationToken)
        {
            var query = new GetSaleQuery(id);
            var result = await _mediator.Send(query, cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Retrieves all sales with optional filtering, pagination and sorting
        /// </summary>
        /// <param name="request">The query parameters</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Paginated list of sales</returns>
        /// <response code="200">Sales retrieved successfully</response>
        [HttpGet]
        [ProducesResponseType(typeof(Common.PaginatedList<SaleDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSales(
            [FromQuery] GetSalesRequest request,
            CancellationToken cancellationToken)
        {
            var query = new GetSalesQuery
            {
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                CustomerId = request.CustomerId,
                BranchId = request.BranchId,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Status = request.Status,
                OrderBy = request.OrderBy ?? "SaleDate",
                Ascending = request.Ascending
            };

            var result = await _mediator.Send(query, cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Updates an existing sale
        /// </summary>
        /// <param name="id">The sale ID</param>
        /// <param name="request">The sale update request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The updated sale</returns>
        /// <response code="200">Sale updated successfully</response>
        /// <response code="400">Invalid request data</response>
        /// <response code="404">Sale not found</response>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(SaleDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateSale(
            [FromRoute] Guid id,
            [FromBody] UpdateSaleRequest request,
            CancellationToken cancellationToken)
        {
            var command = new UpdateSaleCommand
            {
                Id = id,
                CustomerId = request.CustomerId,
                CustomerName = request.CustomerName,
                BranchId = request.BranchId,
                BranchName = request.BranchName,
                Items = request.Items.Select(i => new UpdateSaleItemCommand
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    ProductName = i.ProductName,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            };

            var result = await _mediator.Send(command, cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Cancels a sale
        /// </summary>
        /// <param name="id">The sale ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The cancelled sale</returns>
        /// <response code="200">Sale cancelled successfully</response>
        /// <response code="400">Sale already cancelled</response>
        /// <response code="404">Sale not found</response>
        [HttpPatch("{id:guid}/cancel")]
        [ProducesResponseType(typeof(SaleDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CancelSale(
            [FromRoute] Guid id,
            CancellationToken cancellationToken)
        {
            var command = new CancelSaleCommand(id);
            var result = await _mediator.Send(command, cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Cancels a specific item in a sale
        /// </summary>
        /// <param name="id">The sale ID</param>
        /// <param name="itemId">The item ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The updated sale</returns>
        /// <response code="200">Item cancelled successfully</response>
        /// <response code="400">Item already cancelled or sale is cancelled</response>
        /// <response code="404">Sale or item not found</response>
        [HttpPatch("{id:guid}/items/{itemId:guid}/cancel")]
        [ProducesResponseType(typeof(SaleDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CancelItem(
            [FromRoute] Guid id,
            [FromRoute] Guid itemId,
            CancellationToken cancellationToken)
        {
            var command = new CancelItemCommand(id, itemId);
            var result = await _mediator.Send(command, cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Deletes a sale permanently
        /// </summary>
        /// <param name="id">The sale ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>No content</returns>
        /// <response code="204">Sale deleted successfully</response>
        /// <response code="404">Sale not found</response>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteSale(
            [FromRoute] Guid id,
            CancellationToken cancellationToken)
        {
            var command = new DeleteSaleCommand(id);
            await _mediator.Send(command, cancellationToken);

            return NoContent();
        }
    }
}

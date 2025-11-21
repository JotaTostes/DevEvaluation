using System.ComponentModel.DataAnnotations;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Users.GetUser;

public class GetSalesRequest
{
 
    [Range(1, int.MaxValue, ErrorMessage = "Page number must be at least 1")]
    public int PageNumber { get; set; } = 1;

    [Range(1, 100, ErrorMessage = "Page size must be between 1 and 100")]
    public int PageSize { get; set; } = 10;

    public string? CustomerId { get; set; }

    public string? BranchId { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

   
    [Range(1, 2, ErrorMessage = "Status must be 1 (Active) or 2 (Cancelled)")]
    public int? Status { get; set; }

    public string? OrderBy { get; set; }
    public bool Ascending { get; set; } = false;
}

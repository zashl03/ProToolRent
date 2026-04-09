
namespace ProToolRent.Application.DTOs;
public record OrderSummaryDto
(
    Guid OrderId,
    Guid ToolId,
    string ToolName,
    string ToolBrand,
    string? ToolImageUrl,
    decimal ToolPricePerDay,
    int ToolQuantity,
    DateOnly StartDate,
    DateOnly EndDate,
    decimal TotalPrice,
    string Status,
    DateTime CreatedAt,
    string? TenantName,
    string? LandlordName
);
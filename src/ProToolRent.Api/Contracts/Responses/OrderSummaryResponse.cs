using ProToolRent.Application.DTOs;

namespace ProToolRent.Api.Contracts.Responses;
public record OrderSummaryResponse
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
)
{
    public static OrderSummaryResponse FromDto(OrderSummaryDto dto)
    {
        return new OrderSummaryResponse(
            dto.OrderId,
            dto.ToolId,
            dto.ToolName,
            dto.ToolBrand,
            dto.ToolImageUrl,
            dto.ToolPricePerDay,
            dto.ToolQuantity,
            dto.StartDate,
            dto.EndDate,
            dto.TotalPrice,
            dto.Status,
            dto.CreatedAt,
            dto.TenantName,
            dto.LandlordName
        );
    }

    public static List<OrderSummaryResponse> FromDtoList(List<OrderSummaryDto> dtoList)
    {
        return dtoList.Select(OrderSummaryResponse.FromDto).ToList();
    }
}

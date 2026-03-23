using ProToolRent.Application.DTOs;

namespace ProToolRent.Api.Contracts.Responses;

public record OrderItemResponse
(
    Guid Id,
    DateTime CreatedDate,
    DateTime EndDate,
    decimal Cost,
    int Quantity,
    Guid ToolId
)
{
    public static OrderItemResponse FromDto(OrderItemDto dto)
    {
        return new OrderItemResponse(
            dto.Id,
            dto.CreatedDate,
            dto.EndDate,
            dto.Cost,
            dto.Quantity,
            dto.ToolId);
    }
}

public record CreateOrderItemResponse(Guid id);
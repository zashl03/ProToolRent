using ProToolRent.Application.DTOs;

namespace ProToolRent.Api.Contracts.Responses;

public record OrderResponse
(
    Guid Id,
    string Status,
    DateTime CreatedDate,
    DateTime EndDate,
    List<OrderItemDto> OrderItems
)
{
    public static OrderResponse FromDto(OrderDto dto)
    {
        return new OrderResponse(
            dto.Id,
            dto.Status, 
            dto.CreatedDate, 
            dto.EndDate, 
            dto.OrderItems
            );
    }
}

public record CreateOrderResponse(Guid id);
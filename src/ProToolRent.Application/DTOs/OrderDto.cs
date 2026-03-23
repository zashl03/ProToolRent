
namespace ProToolRent.Application.DTOs;

public record OrderDto
(
    Guid Id,
    string Status,
    DateTime CreatedDate,
    DateTime EndDate,
    List<OrderItemDto> OrderItems
)
{ }

namespace ProToolRent.Application.DTOs;

public record OrderItemDto
(
    Guid Id,
    DateTime CreatedDate,
    DateTime EndDate,
    decimal Cost,
    int Quantity,
    Guid ToolId
)
{
}

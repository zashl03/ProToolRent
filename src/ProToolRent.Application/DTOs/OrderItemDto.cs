namespace ProToolRent.Application.DTOs;

public record OrderItemDto
(
    Guid Id,
    DateOnly CreatedDate,
    DateOnly EndDate,
    decimal Cost,
    int Quantity,
    Guid ToolId
)
{
}

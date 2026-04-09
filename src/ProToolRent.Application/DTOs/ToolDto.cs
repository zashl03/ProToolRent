using ProToolRent.Domain.Entities;

namespace ProToolRent.Application.DTOs;

public record ToolDto
(
    Guid Id,
    string Brand,
    string Name,
    double Power,
    string Description,
    int AvailableQuantity,
    decimal Price,
    Guid CategoryId,
    Guid UserId,
    string? ImageUrl
)
{
    public static ToolDto FromEntity(Tool tool)
    {
        return new ToolDto(
            tool.Id,
            tool.Specification.Brand,
            tool.Specification.Name,
            tool.Specification.Power,
            tool.Description,
            tool.Quantity.Available,
            tool.Price,
            tool.CategoryId,
            tool.UserId,
            tool.ImageUrl);
    }
}


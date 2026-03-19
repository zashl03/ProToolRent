using ProToolRent.Application.DTOs;

namespace ProToolRent.Api.Contracts.Responses;

public record ToolResponse
(
    Guid Id,
    string Brand,
    string Name,
    double Power,
    string Description,
    string Status,
    double Price,
    Guid CategoryId,
    Guid UserId
)
{
    public static ToolResponse FromDto(ToolDto dto) => new(
        dto.Id, dto.Brand, dto.Name, dto.Power, dto.Description,
        dto.Status, dto.Price, dto.CategoryId, dto.UserId
        );
}
public record CreateToolResponse(Guid id);
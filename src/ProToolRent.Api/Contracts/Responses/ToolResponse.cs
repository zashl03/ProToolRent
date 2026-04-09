using ProToolRent.Application.DTOs;

namespace ProToolRent.Api.Contracts.Responses;

public record ToolResponse
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
    public static ToolResponse FromDto(ToolDto dto) => new(
        dto.Id, 
        dto.Brand, 
        dto.Name, 
        dto.Power, 
        dto.Description,
        dto.AvailableQuantity,
        dto.Price, 
        dto.CategoryId, 
        dto.UserId, 
        dto.ImageUrl
        );

    public static List<ToolResponse> FromDtoList(List<ToolDto> dtoList)
    {
        var list = new List<ToolResponse>();
        foreach(ToolDto dto in dtoList)
        {
            list.Add(FromDto(dto));
        }
        return list;
    }
}
public record CreateToolResponse(Guid id);
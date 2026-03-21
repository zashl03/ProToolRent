using ProToolRent.Application.DTOs;

namespace ProToolRent.Api.Contracts.Responses;

public record CategoryResponse
(
    Guid Id,
    string Name,
    Guid? ParentId
)
{
    public static CategoryResponse FromDto(CategoryDto dto)
    {
        return new CategoryResponse(
            dto.Id,
            dto.Name,
            dto.ParentId);
    }
}

public record CreateCategoryResponse(Guid id);
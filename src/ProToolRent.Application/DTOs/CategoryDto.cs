using ProToolRent.Domain.Entities;

namespace ProToolRent.Application.DTOs;
public record CategoryDto
(
    Guid Id,
    string Name,
    Guid? ParentId
)
{
    public static CategoryDto FromEntity(Category category)
    {
        return new CategoryDto(
            category.Id,
            category.Name,
            category.ParentId);
    }
}

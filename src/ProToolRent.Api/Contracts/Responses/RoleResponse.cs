using ProToolRent.Application.DTOs;

namespace ProToolRent.Api.Contracts.Responses;

public record RoleResponse
(
    Guid Id,
    string Name
)
{
    public static RoleResponse FromDto(RoleDto dto)
    {
        return new RoleResponse(
            dto.Id,
            dto.Name);
    }
}

public record CreateRoleResponse(Guid id);
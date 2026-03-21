using MediatR;
using ProToolRent.Application.Common;
using ProToolRent.Domain.Entities;

namespace ProToolRent.Application.DTOs;

public record RoleDto
(
    Guid Id,
    string Name
)
{
    public static RoleDto FromEntity(Role role)
    {
        return new RoleDto(
            role.Id,
            role.Name);
    }
}
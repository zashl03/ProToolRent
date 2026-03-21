using ProToolRent.Domain.Entities;

namespace ProToolRent.Application.DTOs;

public record UserDto
(
    Guid Id,
    string Fullname,
    string Organization,
    string City,
    Guid RoleId
)
{
    public static UserDto FromEntity(User user)
    {
        return new UserDto(
            user.Id,
            user.Fullname,
            user.Organization,
            user.City,
            user.RoleId);
    }
}
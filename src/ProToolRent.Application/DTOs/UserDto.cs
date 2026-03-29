using ProToolRent.Domain.Entities;

namespace ProToolRent.Application.DTOs;

public record UserDto
(
    Guid Id,
    string Email,
    string? FirstName,
    string? LastName,
    string? City,
    string? Organization,
    string? Phone,
    string Role
)
{
    public static UserDto FromEntity(User user)
    {
        return new UserDto(
            user.Id,
            user.Email,
            user.Profile.FirstName,
            user.Profile.LastName,
            user.Profile.City,
            user.Profile.Organization,
            user.Profile.Phone,
            user.Role.ToString());
    }
}
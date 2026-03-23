using ProToolRent.Domain.Entities;

namespace ProToolRent.Application.DTOs;

public record UserDto
(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string City,
    string Organization,
    string Phone,
    Guid RoleId
)
{
    public static UserDto FromEntity(User user, UserProfile userProfile)
    {
        return new UserDto(
            user.Id,
            user.Email,
            userProfile.FirstName,
            userProfile.LastName,
            userProfile.City,
            userProfile.Organization,
            userProfile.Phone,
            user.RoleId);
    }
}
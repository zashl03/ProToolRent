using ProToolRent.Application.DTOs;

namespace ProToolRent.Api.Contracts.Responses;

public record UserResponse
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
    public static UserResponse FromDto(UserDto dto)
    {
        return new UserResponse(
            dto.Id,
            dto.Email,
            dto.FirstName,
            dto.LastName,
            dto.City,
            dto.Organization,
            dto.Phone,
            dto.RoleId);
    }
}

public record CreateUserResponse(Guid id);
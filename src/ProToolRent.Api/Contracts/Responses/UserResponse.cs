using ProToolRent.Application.DTOs;

namespace ProToolRent.Api.Contracts.Responses;

public record UserResponse
(
    Guid Id,
    string Fullname,
    string Organization,
    string City,
    Guid RoleId
)
{
    public static UserResponse FromDto(UserDto dto)
    {
        return new UserResponse(
            dto.Id,
            dto.Fullname,
            dto.Organization,
            dto.City,
            dto.RoleId);
    }
}

public record CreateUserResponse(Guid id);
namespace ProToolRent.Api.Contracts.Requests;

public record CreateUserRequest
(
    string Email,
    string PasswordHash,
    string FirstName,
    string LastName,
    string City,
    string Organization,
    string Phone,
    Guid RoleId
);

namespace ProToolRent.Api.Contracts.Requests;

public record CreateUserRequest
(
    string Fullname,
    string Organization,
    string City,
    Guid RoleId
);

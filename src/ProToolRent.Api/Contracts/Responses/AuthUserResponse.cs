namespace ProToolRent.Api.Contracts.Responses;

public record AuthUserResponse
(
    string AccessToken,
    Guid UserId,
    string Role
);

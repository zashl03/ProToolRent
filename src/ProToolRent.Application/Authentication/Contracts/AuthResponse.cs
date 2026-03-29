namespace ProToolRent.Application.Authentication.Contracts;

public record AuthResponse
(
    Guid UserId,
    string AccessToken,
    string RefreshToken,
    string Role
);

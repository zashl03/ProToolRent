
namespace ProToolRent.Application.Authentication.Contracts;

public record TokenResponse
(
    string AccessToken,
    string RefreshToken,
    string Role
);

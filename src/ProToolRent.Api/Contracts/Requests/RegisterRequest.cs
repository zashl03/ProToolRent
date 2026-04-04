namespace ProToolRent.Api.Contracts.Requests;

public record RegisterRequest
(
    string Email,
    string Password,
    string RepeatPassword,
    string Role
);

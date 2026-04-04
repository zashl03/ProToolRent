using MediatR;
using ProToolRent.Application.Authentication.Contracts;
using ProToolRent.Application.Common;

namespace ProToolRent.Application.Authentication.Commands.Register;

public record RegisterCommand
(
    string Email,
    string Password,
    string RepeatPassword,
    string Role
) : IRequest<Result<AuthResponse>>;

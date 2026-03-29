using MediatR;
using ProToolRent.Application.Authentication.Contracts;
using ProToolRent.Application.Common;

namespace ProToolRent.Application.Authentication.Commands.Login;

public record LoginCommand
(
    string Email,
    string Password
) : IRequest<Result<AuthResponse>>;

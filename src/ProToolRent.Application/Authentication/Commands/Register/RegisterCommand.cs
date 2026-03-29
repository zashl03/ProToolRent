using MediatR;
using ProToolRent.Application.Authentication.Contracts;
using ProToolRent.Application.Common;
using ProToolRent.Domain.Entities;

namespace ProToolRent.Application.Authentication.Commands.Register;

public record RegisterCommand
(
    string Email,
    string Password,
    string Role
) : IRequest<Result<AuthResponse>>;

using MediatR;
using ProToolRent.Application.Common;

namespace ProToolRent.Application.Authentication.Commands.Logout;

public record LogoutCommand(
    string RefreshToken
    ) : IRequest<Result>;

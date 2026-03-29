using MediatR;
using ProToolRent.Application.Authentication.Contracts;
using ProToolRent.Application.Common;

namespace ProToolRent.Application.Authentication.Commands.Refresh;

public record RefreshCommand
(
    string RefreshToken
) : IRequest<Result<TokenResponse>>;

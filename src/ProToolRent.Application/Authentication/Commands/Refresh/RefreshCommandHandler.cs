

using MediatR;
using ProToolRent.Application.Authentication.Contracts;
using ProToolRent.Application.Common;
using ProToolRent.Application.Interfaces;
using ProToolRent.Domain.Interfaces;

namespace ProToolRent.Application.Authentication.Commands.Refresh;

public class RefreshCommandHandler : IRequestHandler<RefreshCommand, Result<TokenResponse>>
{
    private readonly IJwtProvider _jwtProvider;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RefreshCommandHandler(IJwtProvider jwtProvider, IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _jwtProvider = jwtProvider;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<TokenResponse>> Handle(RefreshCommand request, CancellationToken ct)
    {
        var user = await _userRepository.GetByRefreshTokenAsync(request.RefreshToken, ct);

        if (user == null || user.RefreshTokenExpiresAt <= DateTime.UtcNow)
        {
            return Result<TokenResponse>.Failure("Invalid refresh token");
        }

        var accessToken = _jwtProvider.GenerateAccessToken(user);
        var refreshToken = _jwtProvider.GenerateRefreshToken();

        user.SetRefreshToken(refreshToken, DateTime.UtcNow.AddDays(7));

        await _unitOfWork.SaveChangeAsync(ct);

        return Result<TokenResponse>.Success(new TokenResponse(accessToken, refreshToken));
    }
}

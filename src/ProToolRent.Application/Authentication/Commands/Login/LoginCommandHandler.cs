

using MediatR;
using ProToolRent.Application.Authentication.Contracts;
using ProToolRent.Application.Common;
using ProToolRent.Application.Interfaces;
using ProToolRent.Domain.Interfaces;

namespace ProToolRent.Application.Authentication.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResponse>>
{
    private readonly IJwtProvider _jwtProvider;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public LoginCommandHandler(
        IJwtProvider jwtProvider, 
        IPasswordHasher passwordHasher, 
        IUserRepository userRepository, 
        IUnitOfWork unitOfWork)
    {
        _jwtProvider = jwtProvider;
        _passwordHasher = passwordHasher;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AuthResponse>> Handle(LoginCommand request, CancellationToken ct)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, ct);
        if (user == null)
        {
            return Result<AuthResponse>.NotFound("Invalid input data");
        }

        var passwordCheck = _passwordHasher.Verify(request.Password, user.PasswordHash);
        if(!passwordCheck)
        {
            return Result<AuthResponse>.Failure("Invalid input data");
        }

        var accessToken = _jwtProvider.GenerateAccessToken(user);
        var refreshToken = _jwtProvider.GenerateRefreshToken();

        user.SetRefreshToken(refreshToken, DateTime.UtcNow.AddDays(7));

        await _unitOfWork.SaveChangeAsync(ct);

        var authResponse = new AuthResponse(
            user.Id,
            accessToken,
            refreshToken,
            user.Role.ToString());

        return Result<AuthResponse>.Success(authResponse);
    }
}

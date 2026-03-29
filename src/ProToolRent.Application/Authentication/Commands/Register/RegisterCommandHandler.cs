

using MediatR;
using ProToolRent.Application.Authentication.Contracts;
using ProToolRent.Application.Common;
using ProToolRent.Application.Interfaces;
using ProToolRent.Domain.Entities;
using ProToolRent.Domain.Enums;
using ProToolRent.Domain.Interfaces;

namespace ProToolRent.Application.Authentication.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<AuthResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtProvider _jwtProvider;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterCommandHandler(
        IUserRepository userRepository, 
        IPasswordHasher passwordHasher,
        IJwtProvider jwtProvider,
        IUnitOfWork unitOfWork)
    {  
        _userRepository = userRepository; 
        _passwordHasher = passwordHasher;
        _jwtProvider = jwtProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AuthResponse>> Handle(RegisterCommand request, CancellationToken ct)
    {
        var isUserExist = await _userRepository.GetByEmailAsync(request.Email, ct);
        if (isUserExist != null)
        {
            return Result<AuthResponse>.Conflict($"User with {request.Email} exists");
        }

        if (!Enum.TryParse<UserRole>(request.Role, true, out var userRole))
        {
            return Result<AuthResponse>.NotFound("Role not found");
        }

        var passwordHash = _passwordHasher.Generate(request.Password);
        
        var user = new User(
            request.Email,
            passwordHash,
            userRole);

        var userProfile = UserProfile.CreateEmpty();
        user.SetProfile(userProfile);

        var accessToken = _jwtProvider.GenerateAccessToken(user);
        var refreshToken = _jwtProvider.GenerateRefreshToken();

        user.SetRefreshToken(refreshToken, DateTime.UtcNow.AddDays(7));

        await _userRepository.AddAsync(user, ct);
        await _unitOfWork.SaveChangeAsync(ct);

        var authResponse = new AuthResponse(user.Id, accessToken, refreshToken, request.Role);

        return Result<AuthResponse>.Success(authResponse);
    }
}

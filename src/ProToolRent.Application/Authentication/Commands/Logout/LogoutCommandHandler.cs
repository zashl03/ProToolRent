
using MediatR;
using ProToolRent.Application.Common;
using ProToolRent.Domain.Interfaces;

namespace ProToolRent.Application.Authentication.Commands.Logout;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public LogoutCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(LogoutCommand request, CancellationToken ct)
    {
        var user = await _userRepository.GetByRefreshTokenAsync(request.RefreshToken, ct);

        if (user == null)
        {
            return Result.NotFound("User not found");
        }

        user.ResetRefreshToken();
        await _unitOfWork.SaveChangeAsync(ct);

        return Result.Success();
    }
}

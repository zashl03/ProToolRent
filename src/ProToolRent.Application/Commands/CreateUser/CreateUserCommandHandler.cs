using MediatR;
using ProToolRent.Application.Common;
using ProToolRent.Application.Interfaces;
using ProToolRent.Domain.Entities;
using ProToolRent.Domain.Interfaces;

namespace ProToolRent.Application.Commands.CreateUser;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<Guid>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;

    public CreateUserCommandHandler(
        IUserRepository userRepository, 
        IPasswordHasher passwordHasher, 
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle (CreateUserCommand request, CancellationToken ct)
    {
        var passwordHash = _passwordHasher.Generate(request.Password);

        var user = new User(request.Email, passwordHash, request.Role);
        var userProfile = new UserProfile(
            request.FirstName, 
            request.LastName,
            request.City, 
            request.Organization,
            request.Phone);

        user.SetProfile(userProfile);

        await _userRepository.AddAsync(user, ct);

        await _unitOfWork.SaveChangeAsync(ct);

        return Result<Guid>.Success(userProfile.Id);
    }
}

using MediatR;
using ProToolRent.Application.Common;
using ProToolRent.Domain.Entities;
using ProToolRent.Domain.Interfaces;

namespace ProToolRent.Application.Commands.CreateUser;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<Guid>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateUserCommandHandler(IUserRepository userRepository, IUserProfileRepository userProfileRepository, 
        IRoleRepository roleRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _userProfileRepository = userProfileRepository;
        _roleRepository = roleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle (CreateUserCommand request, CancellationToken ct)
    {
        var role = await _roleRepository.GetByIdAsync(request.RoleId, ct);
        if(role == null)
        {
            return Result<Guid>.NotFound("Current Role does not exist");
        }

        var user = new User(request.Email, request.PasswordHash, request.RoleId);
        var userProfile = new UserProfile(request.FirstName, request.LastName,
            request.City, request.Organization, request.Phone, user.Id);

        await _userRepository.AddAsync(user, ct);
        await _userProfileRepository.AddAsync(userProfile, ct);

        await _unitOfWork.SaveChangeAsync(ct);

        return Result<Guid>.Success(userProfile.Id);
    }
}

using MediatR;
using ProToolRent.Application.Common;
using ProToolRent.Domain.Entities;
using ProToolRent.Domain.Interfaces;

namespace ProToolRent.Application.Commands.CreateUser;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<Guid>>
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;

    public CreateUserCommandHandler(IUserRepository userRepository, IRoleRepository roleRepository)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
    }

    public async Task<Result<Guid>> Handle (CreateUserCommand request, CancellationToken ct)
    {
        var role = await _roleRepository.GetByIdAsync (request.RoleId, ct);
        if(role == null)
        {
            return Result<Guid>.Failure("Current Role does not exist");
        }

        var user = new User(request.Fullname, request.Organization, request.City, request.RoleId);
        await _userRepository.AddAsync(user, ct);

        return Result<Guid>.Success(user.Id);
    }
}

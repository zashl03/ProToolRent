using MediatR;
using ProToolRent.Application.Common;
using ProToolRent.Application.DTOs;
using ProToolRent.Domain.Interfaces;

namespace ProToolRent.Application.Queries.GetRoleById;

public class GetRoleByIdQueryHandler : IRequestHandler<GetRoleByIdQuery, Result<string>>
{
    private readonly IUserRepository _userRepository;

    public GetRoleByIdQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<string>> Handle(GetRoleByIdQuery request, CancellationToken ct)
    {
        var user = await _userRepository.GetByIdAsync(request.Id, ct);

        if (user == null)
        {
            return Result<string>.NotFound($"Role of user with {request.Id} not found");
        }

        return Result<string>.Success(user.Role.ToString());
    }
}

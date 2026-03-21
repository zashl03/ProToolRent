using MediatR;
using ProToolRent.Application.Common;
using ProToolRent.Application.DTOs;
using ProToolRent.Domain.Interfaces;

namespace ProToolRent.Application.Queries.GetUserById;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result<UserDto>>
{
    private readonly IUserRepository _userRepository;

    public GetUserByIdQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<UserDto>> Handle(GetUserByIdQuery request, CancellationToken ct)
    {
        var user = await _userRepository.GetByIdAsync(request.Id);

        if (user == null)
        {
            return Result<UserDto>.NotFound($"User with {request.Id} not found");
        }

        return Result<UserDto>.Success(UserDto.FromEntity(user));
    }
}

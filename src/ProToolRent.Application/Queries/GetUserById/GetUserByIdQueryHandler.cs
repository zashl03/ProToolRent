using MediatR;
using ProToolRent.Application.Common;
using ProToolRent.Application.DTOs;
using ProToolRent.Domain.Interfaces;

namespace ProToolRent.Application.Queries.GetUserById;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result<UserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserProfileRepository _userProfileRepository;

    public GetUserByIdQueryHandler(IUserRepository userRepository, IUserProfileRepository userProfileRepository)
    {
        _userRepository = userRepository;
        _userProfileRepository = userProfileRepository;
    }

    public async Task<Result<UserDto>> Handle(GetUserByIdQuery request, CancellationToken ct)
    {
        var userProfile = await _userProfileRepository.GetByIdAsync(request.UserProfileId);

        if (userProfile == null)
        {
            return Result<UserDto>.NotFound($"User with {request.UserProfileId} not found");
        }

        var user = await _userRepository.GetByIdAsync(userProfile.UserId, ct);

        return Result<UserDto>.Success(UserDto.FromEntity(user!, userProfile));
    }
}

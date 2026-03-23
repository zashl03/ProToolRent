using MediatR;
using ProToolRent.Application.Common;
using ProToolRent.Application.DTOs;

namespace ProToolRent.Application.Queries.GetUserById;

public record GetUserByIdQuery(Guid UserProfileId) : IRequest<Result<UserDto>>;

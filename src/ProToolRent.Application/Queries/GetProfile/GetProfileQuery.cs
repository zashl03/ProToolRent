using MediatR;
using ProToolRent.Application.Common;
using ProToolRent.Application.DTOs;

public record GetProfileQuery
(
    Guid UserId
): IRequest<Result<UserDto>>;
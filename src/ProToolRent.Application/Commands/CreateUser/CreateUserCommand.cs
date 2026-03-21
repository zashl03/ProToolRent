using MediatR;
using ProToolRent.Application.Common;

namespace ProToolRent.Application.Commands.CreateUser;

public record CreateUserCommand
(
    string Fullname,
    string Organization,
    string City,
    Guid RoleId
) : IRequest<Result<Guid>>;

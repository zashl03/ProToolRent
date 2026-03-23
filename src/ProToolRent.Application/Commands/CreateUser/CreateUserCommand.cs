using MediatR;
using ProToolRent.Application.Common;

namespace ProToolRent.Application.Commands.CreateUser;

public record CreateUserCommand
(
    string Email,
    string PasswordHash,
    string FirstName,
    string LastName,
    string City,
    string Organization,
    string Phone,
    Guid RoleId
) : IRequest<Result<Guid>>;

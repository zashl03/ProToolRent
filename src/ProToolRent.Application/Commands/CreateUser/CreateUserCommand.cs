using MediatR;
using ProToolRent.Application.Common;
using ProToolRent.Application.DTOs;
using ProToolRent.Domain.Entities;
using ProToolRent.Domain.Enums;

namespace ProToolRent.Application.Commands.CreateUser;

public record CreateUserCommand
(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    string City,
    string Organization,
    string Phone,
    UserRole Role
) : IRequest<Result<Guid>>;

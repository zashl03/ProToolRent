using MediatR;
using ProToolRent.Application.Common;

public record UpdateProfileCommand
(
    Guid UserId,
    string FirstName,
    string LastName,
    string City,
    string Organization,
    string Phone
) : IRequest<Result>;
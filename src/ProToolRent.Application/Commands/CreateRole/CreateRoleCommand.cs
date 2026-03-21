using MediatR;
using ProToolRent.Application.Common;

namespace ProToolRent.Application.Commands.CreateRole;

public record CreateRoleCommand
(
    string Name
) : IRequest<Result<Guid>>;

using MediatR;
using ProToolRent.Application.Common;
using ProToolRent.Application.DTOs;

namespace ProToolRent.Application.Queries.GetRoleById;

public record GetRoleByIdQuery(Guid Id) : IRequest<Result<RoleDto>>;

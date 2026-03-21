using MediatR;
using ProToolRent.Application.Common;
using ProToolRent.Application.DTOs;
using ProToolRent.Domain.Interfaces;

namespace ProToolRent.Application.Queries.GetRoleById;

public class GetRoleByIdQueryHandler : IRequestHandler<GetRoleByIdQuery, Result<RoleDto>>
{
    private readonly IRoleRepository _roleRepository;

    public GetRoleByIdQueryHandler(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async Task<Result<RoleDto>> Handle(GetRoleByIdQuery request, CancellationToken ct)
    {
        var role = await _roleRepository.GetByIdAsync(request.Id, ct);

        if (role == null)
        {
            return Result<RoleDto>.NotFound($"Role with {request.Id} not found");
        }

        return Result<RoleDto>.Success(RoleDto.FromEntity(role));
    }
}

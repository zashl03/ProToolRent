using MediatR;
using ProToolRent.Application.Common;
using ProToolRent.Domain.Entities;
using ProToolRent.Domain.Interfaces;

namespace ProToolRent.Application.Commands.CreateRole;

public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, Result<Guid>>
{
    private readonly IRoleRepository _repository;

    public CreateRoleCommandHandler(IRoleRepository repository)
    {
        _repository = repository; 
    }

    public async Task<Result<Guid>> Handle(CreateRoleCommand request, CancellationToken ct)
    {
        var role = new Role(request.Name);

        await _repository.AddAsync(role, ct);

        return Result<Guid>.Success(role.Id);
    }
}

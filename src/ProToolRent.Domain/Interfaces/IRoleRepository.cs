using ProToolRent.Domain.Entities;

namespace ProToolRent.Domain.Interfaces;

public interface IRoleRepository
{
    Task<Role?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(Role role, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}

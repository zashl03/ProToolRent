using ProToolRent.Domain.Entities;

namespace ProToolRent.Domain.Interfaces;

public interface IUserProfileRepository
{
    Task<UserProfile?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(UserProfile userProfile, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}

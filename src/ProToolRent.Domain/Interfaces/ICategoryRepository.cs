using ProToolRent.Domain.Entities;

namespace ProToolRent.Domain.Interfaces;

public interface ICategoryRepository
{
    Task<Category?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(Category category, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
    Task <List<Category>> ListAsync(CancellationToken ct = default);
}

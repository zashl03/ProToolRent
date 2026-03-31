using ProToolRent.Domain.Entities;

namespace ProToolRent.Domain.Interfaces;

public interface IToolRepository
{
    Task<Tool?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<Tool>?> GetToolsByUserAsync (Guid id, CancellationToken ct = default);
    Task<PagedResult<Tool>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken ct = default);
    Task AddAsync(Tool tool, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}

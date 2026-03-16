using ProToolRent.Domain.Entities;

namespace ProToolRent.Domain.Interfaces
{
    public interface IToolRepository
    {
        Task<Tool?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task AddAsync(Tool tool, CancellationToken ct = default);
        Task DeleteAsync(Guid id, CancellationToken ct = default);
    }
}

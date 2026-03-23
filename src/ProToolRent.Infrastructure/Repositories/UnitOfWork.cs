using ProToolRent.Domain.Interfaces;
using ProToolRent.Infrastructure.Persistence;

namespace ProToolRent.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public UnitOfWork(AppDbContext appDbContext)
    {
        _context = appDbContext; 
    }

    public async Task<int> SaveChangeAsync(CancellationToken ct)
    {
        return await _context.SaveChangesAsync(ct);
    }
}

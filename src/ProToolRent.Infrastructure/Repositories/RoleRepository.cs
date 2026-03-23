using Microsoft.EntityFrameworkCore;
using ProToolRent.Domain.Entities;
using ProToolRent.Domain.Interfaces;
using ProToolRent.Infrastructure.Persistence;

namespace ProToolRent.Infrastructure.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly AppDbContext _context;

    public RoleRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Role?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _context.Roles.FirstOrDefaultAsync(r => r.Id == id, ct);
    }

    public async Task AddAsync(Role role, CancellationToken ct)
    {
        await _context.Roles.AddAsync(role, ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        var role = await GetByIdAsync(id, ct);
        if (role != null)
        {
            _context.Roles.Remove(role);
        }
    }
}

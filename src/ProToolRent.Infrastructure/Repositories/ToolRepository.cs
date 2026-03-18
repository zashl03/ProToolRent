using Microsoft.EntityFrameworkCore;
using ProToolRent.Domain.Interfaces;
using ProToolRent.Domain.Entities;
using ProToolRent.Infrastructure.Persistence;

namespace ProToolRent.Infrastructure.Repositories;

public class ToolRepository : IToolRepository
{
    private readonly AppDbContext _context;

    public ToolRepository(AppDbContext context)
    {
        _context = context; 
    }

    public async Task<Tool?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _context.Tools.FirstOrDefaultAsync(t => t.Id == id, ct);
    }

    public async Task AddAsync(Tool tool, CancellationToken ct)
    {
        await _context.Tools.AddAsync(tool, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        var tool = await GetByIdAsync(id, ct);

        if (tool != null)
        {
            _context.Tools.Remove(tool);
            await _context.SaveChangesAsync(ct);
        }
    }
}

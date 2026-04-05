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

    public async Task<List<Tool>?> GetToolsByUserAsync(Guid id, CancellationToken ct)
    {
        return await _context.Tools.Where(t => t.UserId == id).ToListAsync(ct);
    }

    public async Task<PagedResult<Tool>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken ct)
    {
        var totalCount = await _context.Tools.CountAsync(ct);
        var items = await _context.Tools
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PagedResult<Tool>(items, totalCount);
    }

    public async Task<PagedResult<Tool>> GetPagedAsync(Guid id, int pageNumber, int pageSize, CancellationToken ct)
    {
        var totalCount = await _context.Tools.Where(t => t.UserId == id).CountAsync(ct);
        var items = await _context.Tools
            .Where(t => t.UserId == id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PagedResult<Tool>(items, totalCount);
    }

    public async Task AddAsync(Tool tool, CancellationToken ct)
    {
        await _context.Tools.AddAsync(tool, ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        var tool = await GetByIdAsync(id, ct);

        if (tool != null)
        {
            _context.Tools.Remove(tool);
        }
    }
}

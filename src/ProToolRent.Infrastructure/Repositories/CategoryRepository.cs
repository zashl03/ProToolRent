using Microsoft.EntityFrameworkCore;
using ProToolRent.Domain.Interfaces;
using ProToolRent.Domain.Entities;
using ProToolRent.Infrastructure.Persistence;

namespace ProToolRent.Infrastructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _context;

    public CategoryRepository(AppDbContext context)
    {
        _context = context; 
    }

    public async Task<Category?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _context.Categories.FirstOrDefaultAsync(c => c.Id == id, ct);
    }

    public async Task AddAsync(Category category, CancellationToken ct)
    {
        await _context.Categories.AddAsync(category, ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        var category = await GetByIdAsync(id, ct);
        if(category != null)
        {
            _context.Categories.Remove(category);
        }
    }

    public async Task<List<Category>> ListAsync(CancellationToken ct)
    {
        return await _context.Categories.ToListAsync(ct);
    }
}

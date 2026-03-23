using Microsoft.EntityFrameworkCore;
using ProToolRent.Domain.Entities;
using ProToolRent.Domain.Interfaces;
using ProToolRent.Infrastructure.Persistence;

namespace ProToolRent.Infrastructure.Repositories;

public class UserProfileRepository : IUserProfileRepository
{
    private readonly AppDbContext _context;

    public UserProfileRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<UserProfile?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _context.UserProfiles.FirstOrDefaultAsync(u => u.Id == id, ct);
    }

    public async Task AddAsync(UserProfile userProfile, CancellationToken ct)
    {
        await _context.UserProfiles.AddAsync(userProfile, ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        var userProfile = await GetByIdAsync(id, ct);
        if (userProfile != null)
        {
            _context.UserProfiles.Remove(userProfile);
        }
    }
}

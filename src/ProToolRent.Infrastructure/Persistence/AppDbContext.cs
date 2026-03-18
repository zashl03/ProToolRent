using Microsoft.EntityFrameworkCore;
using ProToolRent.Domain.Entities;

namespace ProToolRent.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) 
    { 
    }
    public DbSet<Tool> Tools => Set<Tool>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}

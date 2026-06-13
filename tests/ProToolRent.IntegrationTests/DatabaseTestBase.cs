using Microsoft.EntityFrameworkCore;
using ProToolRent.Infrastructure.Persistence;
using Testcontainers.PostgreSql;

namespace ProToolRent.IntegrationTests;

public class DatabaseTestBase : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder("postgres:18-alpine").Build();
    
    protected AppDbContext DbContext { get; private set; } = null!;

    public async ValueTask InitializeAsync()
    {
        await _container.StartAsync();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(_container.GetConnectionString())
            .Options;
        
        DbContext = new AppDbContext(options);
    
        await DbContext.Database.MigrateAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await DbContext.DisposeAsync();
        await _container.DisposeAsync();
    }

    protected async Task ClearDatabaseAsync()
    {
        DbContext.OrderItems.RemoveRange(DbContext.OrderItems);
        DbContext.Orders.RemoveRange(DbContext.Orders);
        DbContext.Tools.RemoveRange(DbContext.Tools);
        DbContext.UserProfiles.RemoveRange(DbContext.UserProfiles);
        DbContext.Users.RemoveRange(DbContext.Users);
        DbContext.Categories.RemoveRange(DbContext.Categories);
       
        await DbContext.SaveChangesAsync();
    }
}

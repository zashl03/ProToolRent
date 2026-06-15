using Microsoft.EntityFrameworkCore;
using ProToolRent.Domain.Entities;
using ProToolRent.Domain.Enums;
using ProToolRent.Domain.ValueObjects;
using ProToolRent.Infrastructure.Persistence;
using Testcontainers.PostgreSql;

namespace ProToolRent.IntegrationTests;

public class DatabaseTestBase : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder("postgres:18-alpine").Build();
    
    protected AppDbContext DbContext { get; private set; } = null!;

    protected CancellationToken Ct => TestContext.Current.CancellationToken;
    public async ValueTask InitializeAsync()
    {
        await _container.StartAsync();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(_container.GetConnectionString())
            .Options;
        
        DbContext = new AppDbContext(options);
    
        await DbContext.Database.MigrateAsync();
        await ClearDatabaseAsync();
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
       
        await DbContext.SaveChangesAsync(Ct);
    }

    protected async Task<Category> CreateCategoryAsync(string name = "TestCategory")
    {
        var category = new Category(name);
        DbContext.Categories.Add(category);
        await DbContext.SaveChangesAsync(Ct);
        DbContext.ChangeTracker.Clear();
        return category;
    }

    protected async Task<User> CreateUserAsync(string email = "user@test.com", UserRole role = UserRole.Tenant)
    {
        var user = new User(email, "passHash", role);
        user.SetProfile(new UserProfile("name", "last", "city", "org", "1234"));
        DbContext.Users.Add(user);
        await DbContext.SaveChangesAsync(Ct);
        DbContext.ChangeTracker.Clear();
        return user;
    }

    protected async Task<Tool> CreateToolAsync(Category category, User owner)
    {
        var spec = new Specification("brand", "name", 100);
        var quan = new Quantity(10);
        var tool = new Tool(spec, quan, "desc", 100, category.Id, owner.Id, "123");
        DbContext.Tools.Add(tool);
        await DbContext.SaveChangesAsync(Ct);
        DbContext.ChangeTracker.Clear();
        return tool;
    }
}

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProToolRent.Infrastructure.Persistence;
using Testcontainers.PostgreSql;

namespace ProToolRent.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder("postgres:18-alpine").Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
           var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

            if(descriptor != null)
                services.Remove(descriptor);

            _container.StartAsync().GetAwaiter().GetResult();

            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(_container.GetConnectionString())); 
        });
    }

    public HttpClient CreateApiClinet() => CreateClient();

    public override async ValueTask DisposeAsync()
    {
        await _container.DisposeAsync();
        await base.DisposeAsync();
    }
}

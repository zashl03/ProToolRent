using Npgsql;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProToolRent.Domain.Interfaces;
using ProToolRent.Infrastructure.Repositories;
using ProToolRent.Infrastructure.Persistence;


namespace ProToolRent.Infrastructure;

public static class DependecyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

        services.AddScoped<IToolRepository, ToolRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        return services;
    }
}

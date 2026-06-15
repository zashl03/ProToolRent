using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProToolRent.Api.Contracts.Requests;
using ProToolRent.Api.Contracts.Responses;
using ProToolRent.Application.Authentication.Commands.Register;
using ProToolRent.Application.Authentication.Contracts;
using ProToolRent.Infrastructure.Persistence;
using Respawn;
using Respawn.Graph;

namespace ProToolRent.IntegrationTests;

public class ApiTestBase : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
{
    protected readonly CustomWebApplicationFactory Factory;
    protected readonly HttpClient Client;
    protected CancellationToken Ct => TestContext.Current.CancellationToken;
    private static Respawner? _respawner;

    public ApiTestBase(CustomWebApplicationFactory factory)
    {
        Factory = factory;
        Client = factory.CreateClient();
    }

    public async ValueTask InitializeAsync()
    {
        await ResetDatabaseAsync();
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    private async Task ResetDatabaseAsync()
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        var connection = dbContext.Database.GetDbConnection();

        if (connection.State != System.Data.ConnectionState.Open)
        {
            await connection.OpenAsync(Ct);
        }

        if (_respawner == null)
        {
            _respawner = await Respawner.CreateAsync(connection, new RespawnerOptions
            {
                DbAdapter = DbAdapter.Postgres,
                
                TablesToIgnore = new[] 
                { 
                    new Table("public", "__EFMigrationsHistory") 
                }
            });
        }
        await _respawner.ResetAsync(connection);
    }

    protected async Task<T?> GetAsync<T>(string url)
    {
        var response = await Client.GetAsync(url, TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>(
            cancellationToken: TestContext.Current.CancellationToken);
    }

    protected async Task<HttpResponseMessage> GetResponseAsync(string url)
    {
        return await Client.GetAsync(url, TestContext.Current.CancellationToken);
    }

    protected async Task<HttpResponseMessage> PostAsync<T>(string url, T content)
    {
        return await Client.PostAsJsonAsync(url, content, TestContext.Current.CancellationToken);
    }

    protected async Task<HttpResponseMessage> PutAsync<T>(string url, T content)
    {
        return await Client.PutAsJsonAsync(url, content);
    }

    protected async Task<HttpResponseMessage> DeleteAsync(string url)
    {
        return await Client.DeleteAsync(url, TestContext.Current.CancellationToken);
    }

    protected async Task<T?> ReadResponseAsync<T>(HttpResponseMessage response)
    {
        return await response.Content.ReadFromJsonAsync<T>(
            cancellationToken: TestContext.Current.CancellationToken);
    }

    protected async Task<AuthContext> RegisterAndAuthenticateAsync(string role)
    {
        var email = $"{role.ToLower()}_{Guid.NewGuid()}@test.com";
        var password = "Password123!";

        var registerResponse = await Client.PostAsJsonAsync(
            "/api/auth/register",
            new RegisterRequest(email, password, password, role),
            Ct);

        registerResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var authResponse = await ReadResponseAsync<AuthUserResponse>(registerResponse);
        authResponse.Should().NotBeNull();

        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authResponse!.AccessToken);

        return new AuthContext(client, authResponse, email, password);
    }

    protected Task<AuthContext> CreateAdminClientAsync() => RegisterAndAuthenticateAsync("Admin");
    protected Task<AuthContext> CreateTenantClientAsync() => RegisterAndAuthenticateAsync("Tenant");
    protected Task<AuthContext> CreateLandlordClientAsync() => RegisterAndAuthenticateAsync("Landlord");

    protected async Task<Guid> CreateCategoryAsync(HttpClient client, string name, Guid? parentId = null)
    {
        var response = await client.PostAsJsonAsync(
            "/api/categories",
            new CreateCategoryRequest(name, parentId),
            Ct);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);

        var created = await ReadResponseAsync<CreateCategoryResponse>(response);
        created.Should().NotBeNull();
        return created!.id;
    }

    protected async Task<Guid> CreateToolAsync(HttpClient client, Guid categoryId, string name)
    {
        var response = await client.PostAsJsonAsync(
            "/api/tools",
            new CreateToolRequest("Bosch", name, 700, "Test tool", 3, 99m, categoryId),
            Ct);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var created = await ReadResponseAsync<CreateToolResponse>(response);
        created.Should().NotBeNull();
        return created!.id;
    }

    protected sealed record AuthContext(
        HttpClient Client,
        AuthUserResponse Auth,
        string Email,
        string Password);
}

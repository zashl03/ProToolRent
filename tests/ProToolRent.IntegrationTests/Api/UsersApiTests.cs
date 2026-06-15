using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using ProToolRent.Api.Contracts.Requests;
using ProToolRent.Api.Contracts.Responses;

namespace ProToolRent.IntegrationTests;

public class UsersApiTests : ApiTestBase
{
    public UsersApiTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task GetById_WhenUserExists_ShouldReturnUser()
    {
        var tenant = await CreateTenantClientAsync();

        var response = await tenant.Client.GetAsync($"/api/users/{tenant.Auth.UserId}", Ct);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var user = await ReadResponseAsync<UserResponse>(response);
        user.Should().NotBeNull();
        user!.Id.Should().Be(tenant.Auth.UserId);
        user.Email.Should().Be(tenant.Email);
        user.Role.Should().Be("Tenant");
    }

    [Fact]
    public async Task GetById_WhenUserIsAnonymous_ShouldReturnUnauthorized()
    {
        var tenant = await CreateTenantClientAsync();

        var response = await Client.GetAsync($"/api/users/{tenant.Auth.UserId}", Ct);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetById_WhenTenantRequestsAnotherUser_ShouldReturnForbidden()
    {
        var tenant = await CreateTenantClientAsync();
        var otherTenant = await CreateTenantClientAsync();

        var response = await tenant.Client.GetAsync($"/api/users/{otherTenant.Auth.UserId}", Ct);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetProfile_WhenUserUpdatesProfile_ShouldReturnUpdatedValues()
    {
        var tenant = await CreateTenantClientAsync();

        var updateResponse = await tenant.Client.PutAsJsonAsync(
            "/api/users/edit/profile",
            new UpdateProfileRequest("John", "Doe", "Prague", "Acme", "12345"),
            Ct);

        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var profileResponse = await tenant.Client.GetAsync("/api/users/me", Ct);
        profileResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var profile = await ReadResponseAsync<UserResponse>(profileResponse);
        profile.Should().NotBeNull();
        profile!.FirstName.Should().Be("John");
        profile.LastName.Should().Be("Doe");
        profile.City.Should().Be("Prague");
        profile.Organization.Should().Be("Acme");
        profile.Phone.Should().Be("12345");
    }

    [Fact]
    public async Task Create_WhenAdminIsAuthorized_ShouldCreateUser()
    {
        var admin = await CreateAdminClientAsync();
        var email = $"created_{Guid.NewGuid()}@test.com";

        var response = await admin.Client.PostAsJsonAsync(
            "/api/users",
            new CreateUserRequest(
                email,
                "hashed-password",
                "Jane",
                "Smith",
                "Berlin",
                "Office",
                "555-12-34",
                "Tenant"),
            Ct);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var created = await ReadResponseAsync<CreateUserResponse>(response);
        created.Should().NotBeNull();

        var userResponse = await admin.Client.GetAsync($"/api/users/{created!.id}", Ct);
        userResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var user = await ReadResponseAsync<UserResponse>(userResponse);
        user.Should().NotBeNull();
        user!.Email.Should().Be(email);
        user.FirstName.Should().Be("Jane");
    }

    [Fact]
    public async Task Create_WhenTenantIsAuthorized_ShouldReturnForbidden()
    {
        var tenant = await CreateTenantClientAsync();
        var email = $"tenant_create_{Guid.NewGuid()}@test.com";

        var response = await tenant.Client.PostAsJsonAsync(
            "/api/users",
            new CreateUserRequest(
                email,
                "hashed-password",
                "Jane",
                "Smith",
                "Berlin",
                "Office",
                "555-12-34",
                "Tenant"),
            Ct);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetById_WhenUserDoesNotExist_ShouldReturnNotFound()
    {
        var admin = await CreateAdminClientAsync();

        var response = await admin.Client.GetAsync($"/api/users/{Guid.NewGuid()}", Ct);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}

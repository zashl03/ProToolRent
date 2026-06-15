using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using ProToolRent.Api.Contracts.Requests;
using ProToolRent.Api.Contracts.Responses;

namespace ProToolRent.IntegrationTests;

public class CategoriesApiTests : ApiTestBase
{
    public CategoriesApiTests(CustomWebApplicationFactory factory) : base(factory)
    {  
    }

    [Fact]
    public async Task List_WhenDatabaseIsEmpty_ShouldReturnEmptyList()
    {
        var categories = await GetAsync<List<CategoryResponse>>("/api/categories");

        categories.Should().NotBeNull();
        categories.Should().BeEmpty();
    }

    [Fact]
    public async Task List_WhenCategoriesExist_ShouldReturnThem()
    {
        var admin = await CreateAdminClientAsync();
        var category = await CreateCategoryAsync(admin.Client, "Test category");

        var categories = await GetAsync<List<CategoryResponse>>("/api/categories");

        categories.Should().NotBeNull();
        categories.Should().ContainSingle(c => c.Name == "Test category");
    }

    [Fact]
    public async Task GetById_WhenCategoryExists_ShouldReturnCategory()
    {
        var admin = await CreateAdminClientAsync();
        var categoryId = await CreateCategoryAsync(admin.Client, "Get by id category");

        var response = await admin.Client.GetAsync($"/api/categories/{categoryId}", Ct);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var category = await ReadResponseAsync<CategoryResponse>(response);
        category.Should().NotBeNull();
        category.Id.Should().Be(categoryId);
        category.Name.Should().Be("Get by id category");
        category.ParentId.Should().BeNull();
    }

    [Fact]
    public async Task GetById_WhenCategoryDoesNotExist_ShouldReturnNotFound()
    {
        var admin = await CreateAdminClientAsync();

        var response = await admin.Client.GetAsync($"/api/categories/{Guid.NewGuid()}", Ct);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Create_WhenRequestIsValid_ShouldCreateCategory()
    {
        var admin = await CreateAdminClientAsync();

        var response = await admin.Client.PostAsJsonAsync(
            "/api/categories",
            new CreateCategoryRequest("Created category", null),
            Ct);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var created = await ReadResponseAsync<CreateCategoryResponse>(response);
        created.Should().NotBeNull();
        created.id.Should().NotBeEmpty();

        var savedCategory = await admin.Client.GetAsync($"/api/categories/{created.id}", Ct);
        savedCategory.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Create_WhenParentCategoryDoesNotExist_ShouldReturnBadRequest()
    {
        var admin = await CreateAdminClientAsync();

        var response = await admin.Client.PostAsJsonAsync(
            "/api/categories",
            new CreateCategoryRequest("Child category", Guid.NewGuid()),
            Ct);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_WhenUserIsAnonymous_ShouldReturnUnauthorized()
    {
        var response = await Client.PostAsJsonAsync(
            "/api/categories",
            new CreateCategoryRequest("Anonymous category", null),
            Ct);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Create_WhenTenantIsAuthorized_ShouldReturnForbidden()
    {
        var tenant = await CreateTenantClientAsync();

        var response = await tenant.Client.PostAsJsonAsync(
            "/api/categories",
            new CreateCategoryRequest("Tenant category", null),
            Ct);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Delete_WhenTenantIsAuthorized_ShouldReturnForbidden()
    {
        var admin = await CreateAdminClientAsync();
        var categoryId = await CreateCategoryAsync(admin.Client, "Tenant cannot delete category");
        var tenant = await CreateTenantClientAsync();

        var response = await tenant.Client.DeleteAsync($"/api/categories/{categoryId}", Ct);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Delete_WhenCategoryExists_ShouldRemoveCategory()
    {
        var admin = await CreateAdminClientAsync();
        var categoryId = await CreateCategoryAsync(admin.Client, "Category to delete");

        var deleteResponse = await admin.Client.DeleteAsync($"/api/categories/{categoryId}", Ct);

        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await admin.Client.GetAsync($"/api/categories/{categoryId}", Ct);
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}

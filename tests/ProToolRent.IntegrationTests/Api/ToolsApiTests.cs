using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using ProToolRent.Api.Contracts.Requests;
using ProToolRent.Api.Contracts.Responses;

namespace ProToolRent.IntegrationTests;

public class ToolsApiTests : ApiTestBase
{
    public ToolsApiTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Create_WhenLandlordIsAuthorized_ShouldCreateTool()
    {
        var context = await CreateLandlordClientAsync();
        var admin = await CreateAdminClientAsync();
        var categoryId = await CreateCategoryAsync(admin.Client, "Category");

        var response = await context.Client.PostAsJsonAsync(
            "/api/tools",
            new CreateToolRequest("Makita", "Drill", 850, "Cordless drill", 5, 120m, categoryId),
            Ct);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var created = await ReadResponseAsync<CreateToolResponse>(response);
        created.Should().NotBeNull();

        var toolResponse = await context.Client.GetAsync($"/api/tools/get-by-id/{created!.id}", Ct);
        toolResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Create_WhenTenantIsAuthorized_ShouldReturnForbidden()
    {
        var tenant = await CreateTenantClientAsync();
        var admin = await CreateAdminClientAsync();
        var categoryId = await CreateCategoryAsync(admin.Client, "Category");

        var response = await tenant.Client.PostAsJsonAsync(
            "/api/tools",
            new CreateToolRequest("Makita", "Tenant drill", 850, "Cordless drill", 5, 120m, categoryId),
            Ct);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetPaged_WhenToolsExist_ShouldReturnThem()
    {
        var landlord = await CreateLandlordClientAsync();
        var admin = await CreateAdminClientAsync();
        var categoryId = await CreateCategoryAsync(admin.Client, "Category");
        await CreateToolAsync(landlord.Client, categoryId, "Paged tool");

        var response = await Client.GetAsync("/api/tools/paged?pageNumber=1&pageSize=10", Ct);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var paged = await ReadResponseAsync<PagedResponse<ToolResponse>>(response);
        paged.Should().NotBeNull();
        paged!.TotalCount.Should().BeGreaterThanOrEqualTo(1);
        paged.Items.Should().ContainSingle(tool => tool.Name == "Paged tool");
    }

    [Fact]
    public async Task GetByOwner_WhenLandlordHasTools_ShouldReturnThem()
    {
        var landlord = await CreateLandlordClientAsync();
        var admin = await CreateAdminClientAsync();
        var categoryId = await CreateCategoryAsync(admin.Client, "Category");
        var toolId = await CreateToolAsync(landlord.Client, categoryId, "Owner tool");

        var response = await landlord.Client.GetAsync($"/api/tools/get-by-owner/{landlord.Auth.UserId}", Ct);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var tools = await ReadResponseAsync<List<ToolResponse>>(response);
        tools.Should().NotBeNull();
        tools!.Should().ContainSingle(tool => tool.Id == toolId && tool.Name == "Owner tool");
    }

    [Fact]
    public async Task GetMy_WhenLandlordHasTools_ShouldReturnOwnTools()
    {
        var landlord = await CreateLandlordClientAsync();
        var admin = await CreateAdminClientAsync();
        var categoryId = await CreateCategoryAsync(admin.Client, "Category");
        await CreateToolAsync(landlord.Client, categoryId, "My tool");

        var response = await landlord.Client.GetAsync("/api/tools/my?pageNumber=1&pageSize=10", Ct);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var paged = await ReadResponseAsync<PagedResponse<ToolResponse>>(response);
        paged.Should().NotBeNull();
        paged!.Items.Should().ContainSingle(tool => tool.Name == "My tool");
    }

    [Fact]
    public async Task GetMy_WhenTenantIsAuthorized_ShouldReturnForbidden()
    {
        var tenant = await CreateTenantClientAsync();

        var response = await tenant.Client.GetAsync("/api/tools/my?pageNumber=1&pageSize=10", Ct);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Delete_WhenToolExists_ShouldRemoveIt()
    {
        var landlord = await CreateLandlordClientAsync();
        var admin = await CreateAdminClientAsync();
        var categoryId = await CreateCategoryAsync(admin.Client, "Category");
        var toolId = await CreateToolAsync(landlord.Client, categoryId, "Tool to delete");

        var deleteResponse = await landlord.Client.DeleteAsync($"/api/tools/{toolId}", Ct);

        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await landlord.Client.GetAsync($"/api/tools/get-by-id/{toolId}", Ct);
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_WhenTenantIsAuthorized_ShouldReturnForbidden()
    {
        var landlord = await CreateLandlordClientAsync();
        var admin = await CreateAdminClientAsync();
        var categoryId = await CreateCategoryAsync(admin.Client, "Category");
        var toolId = await CreateToolAsync(landlord.Client, categoryId, "Tenant cannot delete tool");
        var tenant = await CreateTenantClientAsync();

        var response = await tenant.Client.DeleteAsync($"/api/tools/{toolId}", Ct);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UploadImage_WhenToolExists_ShouldReturnImageUrl()
    {
        var landlord = await CreateLandlordClientAsync();
        var admin = await CreateAdminClientAsync();
        var categoryId = await CreateCategoryAsync(admin.Client, "Category");
        var toolId = await CreateToolAsync(landlord.Client, categoryId, "Upload tool");

        using var content = new MultipartFormDataContent();
        using var fileContent = new ByteArrayContent(new byte[] { 1, 2, 3 });
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/png");
        content.Add(fileContent, "file", "tool.png");

        var response = await landlord.Client.PostAsync($"/api/tools/{toolId}/image", content, Ct);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var upload = await ReadResponseAsync<UploadImageResponse>(response);
        upload.Should().NotBeNull();
        upload!.ImageUrl.Should().Contain($"/images/tools/{toolId}.png");
    }

    [Fact]
    public async Task UploadImage_WhenUserIsAnonymous_ShouldReturnUnauthorized()
    {
        var landlord = await CreateLandlordClientAsync();
        var admin = await CreateAdminClientAsync();
        var categoryId = await CreateCategoryAsync(admin.Client, "Category");
        var toolId = await CreateToolAsync(landlord.Client, categoryId, "Anonymous upload tool");

        using var content = CreateImageContent();

        var response = await Client.PostAsync($"/api/tools/{toolId}/image", content, Ct);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UploadImage_WhenTenantIsAuthorized_ShouldReturnForbidden()
    {
        var landlord = await CreateLandlordClientAsync();
        var admin = await CreateAdminClientAsync();
        var categoryId = await CreateCategoryAsync(admin.Client, "Category");
        var toolId = await CreateToolAsync(landlord.Client, categoryId, "Tenant upload tool");
        var tenant = await CreateTenantClientAsync();

        using var content = CreateImageContent();

        var response = await tenant.Client.PostAsync($"/api/tools/{toolId}/image", content, Ct);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    private static MultipartFormDataContent CreateImageContent()
    {
        var content = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(new byte[] { 1, 2, 3 });
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/png");
        content.Add(fileContent, "file", "tool.png");
        return content;
    }

    private sealed record PagedResponse<T>(List<T> Items, int TotalCount);

    private sealed record UploadImageResponse(string ImageUrl);
}

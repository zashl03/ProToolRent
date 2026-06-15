using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using ProToolRent.Api.Contracts.Requests;
using ProToolRent.Api.Contracts.Responses;

namespace ProToolRent.IntegrationTests;

public class OrdersApiTests : ApiTestBase
{
    public OrdersApiTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Create_WhenTenantIsAuthorized_ShouldCreateOrder()
    {
        var tenant = await CreateTenantClientAsync();
        var landlord = await CreateLandlordClientAsync();
        var admin = await CreateAdminClientAsync();
        var categoryId = await CreateCategoryAsync(admin.Client, "Category");
        var toolId = await CreateToolAsync(landlord.Client, categoryId, "Order tool");

        var response = await tenant.Client.PostAsJsonAsync(
            "/api/orders",
            new CreateOrderRequest(
                tenant.Auth.UserId,
                toolId,
                DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(1)),
                DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(3)),
                1),
            Ct);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var summary = await ReadResponseAsync<OrderSummaryResponse>(response);
        summary.Should().NotBeNull();
        summary!.ToolId.Should().Be(toolId);
        summary.Status.Should().NotBeNullOrWhiteSpace();

        var orderResponse = await tenant.Client.GetAsync($"/api/orders/{summary.OrderId}", Ct);
        orderResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Create_WhenLandlordIsAuthorized_ShouldReturnForbidden()
    {
        var landlord = await CreateLandlordClientAsync();
        var admin = await CreateAdminClientAsync();
        var categoryId = await CreateCategoryAsync(admin.Client, "Category");
        var toolId = await CreateToolAsync(landlord.Client, categoryId, "Landlord order tool");

        var response = await landlord.Client.PostAsJsonAsync(
            "/api/orders",
            new CreateOrderRequest(
                landlord.Auth.UserId,
                toolId,
                DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(1)),
                DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(3)),
                1),
            Ct);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetMy_WhenTenantHasOrders_ShouldReturnOwnOrders()
    {
        var tenant = await CreateTenantClientAsync();
        var landlord = await CreateLandlordClientAsync();
        var admin = await CreateAdminClientAsync();
        var categoryId = await CreateCategoryAsync(admin.Client, "Category");
        var toolId = await CreateToolAsync(landlord.Client, categoryId, "Summary tool");

        var order = await CreateOrderAsync(tenant.Client, tenant.Auth.UserId, toolId);

        var response = await tenant.Client.GetAsync("/api/orders/my", Ct);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var orders = await ReadResponseAsync<List<OrderSummaryResponse>>(response);
        orders.Should().NotBeNull();
        orders!.Should().ContainSingle(order => order.ToolId == toolId);
    }

    [Fact]
    public async Task GetMy_WhenUserIsAnonymous_ShouldReturnUnauthorized()
    {
        var response = await Client.GetAsync("/api/orders/my", Ct);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetById_WhenUserIsAnonymous_ShouldReturnUnauthorized()
    {
        var tenant = await CreateTenantClientAsync();
        var landlord = await CreateLandlordClientAsync();
        var admin = await CreateAdminClientAsync();
        var categoryId = await CreateCategoryAsync(admin.Client, "Category");
        var toolId = await CreateToolAsync(landlord.Client, categoryId, "Anonymous order lookup tool");
        var orderId = await CreateOrderAsync(tenant.Client, tenant.Auth.UserId, toolId);

        var response = await Client.GetAsync($"/api/orders/{orderId}", Ct);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateOrderItem_WhenRequestIsValid_ShouldCreateItem()
    {
        var tenant = await CreateTenantClientAsync();
        var landlord = await CreateLandlordClientAsync();
        var admin = await CreateAdminClientAsync();
        var categoryId = await CreateCategoryAsync(admin.Client, "Category");
        var toolId = await CreateToolAsync(landlord.Client, categoryId, "Additional item tool");
        var orderId = await CreateOrderAsync(tenant.Client, tenant.Auth.UserId, toolId);
        var additionalToolId = await CreateToolAsync(landlord.Client, categoryId, "Second order item tool");

        var response = await tenant.Client.PostAsJsonAsync(
            $"/api/orders/{orderId}/items",
            new CreateOrderItemRequest(
                orderId,
                additionalToolId,
                DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(3)),
                DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(4)),
                1),
            Ct);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var created = await ReadResponseAsync<CreateOrderItemResponse>(response);
        created.Should().NotBeNull();
        created!.id.Should().NotBeEmpty();

        var orderResponse = await tenant.Client.GetAsync($"/api/orders/{orderId}", Ct);
        orderResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var order = await ReadResponseAsync<OrderResponse>(orderResponse);
        order.Should().NotBeNull();
        order!.OrderItems.Should().Contain(item => item.Id == created.id);
    }

    [Fact]
    public async Task GetOrderItem_WhenItemExists_ShouldReturnItem()
    {
        var tenant = await CreateTenantClientAsync();
        var landlord = await CreateLandlordClientAsync();
        var admin = await CreateAdminClientAsync();
        var categoryId = await CreateCategoryAsync(admin.Client, "Category");
        var toolId = await CreateToolAsync(landlord.Client, categoryId, "Item tool");

        var orderId = await CreateOrderAsync(tenant.Client, tenant.Auth.UserId, toolId);

        var orderResponse = await tenant.Client.GetAsync($"/api/orders/{orderId}", Ct);
        orderResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var itemId = await GetFirstOrderItemIdAsync(tenant.Client, orderId);

        var getItemResponse = await tenant.Client.GetAsync($"/api/orders/{orderId}/items/{itemId}", Ct);
        getItemResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var orderItem = await ReadResponseAsync<OrderItemResponse>(getItemResponse);
        orderItem.Should().NotBeNull();
        orderItem!.ToolId.Should().Be(toolId);

        var orderWithItemsResponse = await tenant.Client.GetAsync($"/api/orders/{orderId}", Ct);
        orderWithItemsResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var order = await ReadResponseAsync<OrderResponse>(orderWithItemsResponse);
        order.Should().NotBeNull();
        order!.OrderItems.Should().ContainSingle(item => item.Id == itemId);
    }

    [Fact]
    public async Task GetOrderItem_WhenUserIsAnonymous_ShouldReturnUnauthorized()
    {
        var tenant = await CreateTenantClientAsync();
        var landlord = await CreateLandlordClientAsync();
        var admin = await CreateAdminClientAsync();
        var categoryId = await CreateCategoryAsync(admin.Client, "Category");
        var toolId = await CreateToolAsync(landlord.Client, categoryId, "Anonymous order item tool");
        var orderId = await CreateOrderAsync(tenant.Client, tenant.Auth.UserId, toolId);
        var itemId = await GetFirstOrderItemIdAsync(tenant.Client, orderId);

        var response = await Client.GetAsync($"/api/orders/{orderId}/items/{itemId}", Ct);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    private async Task<Guid> CreateOrderAsync(HttpClient client, Guid userId, Guid toolId)
    {
        var response = await client.PostAsJsonAsync(
            "/api/orders",
            new CreateOrderRequest(
                userId,
                toolId,
                DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(1)),
                DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(2)),
                1),
            Ct);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var order = await ReadResponseAsync<OrderSummaryResponse>(response);
        order.Should().NotBeNull();
        return order!.OrderId;
    }

    private async Task<Guid> GetFirstOrderItemIdAsync(HttpClient client, Guid orderId)
    {
        var response = await client.GetAsync($"/api/orders/{orderId}", Ct);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var order = await ReadResponseAsync<OrderResponse>(response);
        order.Should().NotBeNull();
        order!.OrderItems.Should().ContainSingle();
        return order.OrderItems.Single().Id;
    }
}

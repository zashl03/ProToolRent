using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ProToolRent.Domain.Entities;
using ProToolRent.Domain.Enums;
using ProToolRent.Domain.ValueObjects;
using ProToolRent.Infrastructure.Repositories;
namespace ProToolRent.IntegrationTests;

public class OrderRepositoryTests : DatabaseTestBase
{
    [Fact]
    public async Task GetByIdAsync_WhenOrderExists_ShouldReturnsIt()
    {
        var repository = new OrderRepository(DbContext);
        var user = await CreateUserAsync();
        var order = new Order(user.Id);

        DbContext.Orders.Add(order);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        DbContext.ChangeTracker.Clear();

        var result = await repository.GetByIdAsync(order.Id, TestContext.Current.CancellationToken);

        result.Should().NotBeNull();
        result.Id.Should().Be(order.Id);
        result.Status.Should().Be("Создан");
    }

    [Fact]
    public async Task GetByIdAsync_WhenOrderDoesNotExists_ShouldReturnsNull()
    {
        var repository = new OrderRepository(DbContext);
        var orderId = Guid.NewGuid();

        var result = await repository.GetByIdAsync(orderId, Ct);

        result.Should().BeNull();
    }

    [Fact]
    public async Task AddAsync_WhenOrderExists_ShouldPersistToDatabase()
    {
        var repository = new OrderRepository(DbContext);
        var user = await CreateUserAsync();
        var order = new Order(user.Id);

        await repository.AddAsync(order, Ct);
        await DbContext.SaveChangesAsync(Ct);

        DbContext.ChangeTracker.Clear();

        var persistedOrder = await DbContext.Orders.FirstOrDefaultAsync(o => o.Id == order.Id, Ct);

        persistedOrder.Should().NotBeNull();
        persistedOrder.Id.Should().Be(order.Id);
        persistedOrder.Status.Should().Be("Создан");
    }

    [Fact]
    public async Task DeleteAsync_WhenOrderExists_ShouldRemoveIt()
    {
        var repository = new OrderRepository(DbContext);
        var user = await CreateUserAsync();
        var order = new Order(user.Id);

        DbContext.Orders.Add(order);
        await DbContext.SaveChangesAsync(Ct);

        DbContext.ChangeTracker.Clear();

        await repository.DeleteAsync(order.Id, Ct);
        await DbContext.SaveChangesAsync(Ct);

        var removedOrder = await DbContext.Orders
            .FirstOrDefaultAsync(c => c.Id == order.Id, Ct);
        
        removedOrder.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WhenOrderDoesNotExists_ShouldNotThrow()
    {
        var repository = new OrderRepository(DbContext);
        var orderId = Guid.NewGuid();

        await repository.DeleteAsync(orderId, Ct);

        var removedOrder = await DbContext.Orders
            .FirstOrDefaultAsync(c => c.Id == orderId, Ct);
        
        removedOrder.Should().BeNull();
    }

    [Fact]
    public async Task GetOrderItemByIdAsync_WhenOrderItemExists_ShouldReturnsIt()
    {
        var repository = new OrderRepository(DbContext);
        var tenant = await CreateUserAsync(email: "tenant@test.com", role: UserRole.Tenant);
        var landlord = await CreateUserAsync(email: "landlord@test.com", role: UserRole.Landlord);
        var category = await CreateCategoryAsync();
        var tool = await CreateToolAsync(category, landlord);

        var order = new Order(tenant.Id);
        var createdDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
        var endDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7));
        var reservedQuantity = 1;
        order.AddItem(createdDate, endDate, reservedQuantity, tool);
        var orderItemId = order.OrderItems.Single().Id;

        DbContext.Attach(tool);

        DbContext.Orders.Add(order);
        await DbContext.SaveChangesAsync(Ct);

        DbContext.ChangeTracker.Clear();

        var result = await repository.GetOrderItemByIdAsync(orderItemId, Ct);

        result.Should().NotBeNull();
        result.Id.Should().Be(orderItemId);
        result.Cost.Should().Be(600);
        result.OrderId.Should().Be(order.Id);
    }

    [Fact]
    public async Task GetOrderItemByIdAsync_WhenOrderItemDoesNotExists_ShouldReturnsNull()
    {
        var repository = new OrderRepository(DbContext);
        var orderItemId = Guid.NewGuid();

        var result = await repository.GetOrderItemByIdAsync(orderItemId, Ct);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetOrdersByTenantAsync_WhenOrdersExist_ShouldReturnsThem()
    {
        var repository = new OrderRepository(DbContext);
        var tenant1 = await CreateUserAsync(email: "tenant1@test.com", role: UserRole.Tenant);
        var tenant2 = await CreateUserAsync(email: "tenant2@test.com", role: UserRole.Tenant);
        var landlord = await CreateUserAsync(email: "landlord@test.com", role: UserRole.Landlord);
        var category = await CreateCategoryAsync();
        var tool = await CreateToolAsync(category, landlord);

        var order1 = new Order(tenant1.Id);
        var order2 = new Order(tenant2.Id);
        var createdDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
        var endDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7));
        var reservedQuantity = 1;
        order1.AddItem(createdDate, endDate, reservedQuantity, tool);

        DbContext.Attach(tool);

        DbContext.Orders.Add(order1);
        DbContext.Orders.Add(order2);
        await DbContext.SaveChangesAsync(Ct);

        DbContext.ChangeTracker.Clear();

        var result = await repository.GetOrdersByTenantAsync(tenant1.Id, Ct);

        result.Should().NotBeNull();
        result.Count.Should().Be(1);
        result.Single().UserId.Should().Be(tenant1.Id);
    }

    [Fact]
    public async Task GetOrdersByLandlordAsync_WhenOrdersExist_ShouldReturnsThem()
    {
        var repository = new OrderRepository(DbContext);
        var tenant1 = await CreateUserAsync(email: "tenant1@test.com", role: UserRole.Tenant);
        var tenant2 = await CreateUserAsync(email: "tenant2@test.com", role: UserRole.Tenant);
        var landlord = await CreateUserAsync(email: "landlord@test.com", role: UserRole.Landlord);
        var category = await CreateCategoryAsync();
        var tool = await CreateToolAsync(category, landlord);

        var order1 = new Order(tenant1.Id);
        var order2 = new Order(tenant2.Id);
        var createdDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
        var endDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7));
        var reservedQuantity = 1;
        order1.AddItem(createdDate, endDate, reservedQuantity, tool);
        order2.AddItem(createdDate, endDate, reservedQuantity, tool);

        DbContext.Attach(tool);

        DbContext.Orders.Add(order1);
        DbContext.Orders.Add(order2);
        await DbContext.SaveChangesAsync(Ct);

        DbContext.ChangeTracker.Clear();

        var result = await repository.GetOrdersByLandlordAsync(landlord.Id, Ct);

        result.Should().NotBeNull();
        result.Count.Should().Be(2);
        result.Should().AllSatisfy(order => 
            order.OrderItems.Single().Tool.UserId.Should().Be(landlord.Id));
    }
}

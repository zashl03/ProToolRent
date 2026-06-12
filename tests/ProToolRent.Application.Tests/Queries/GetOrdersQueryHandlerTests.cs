using FluentAssertions;
using Moq;
using ProToolRent.Application.Common;
using ProToolRent.Application.Queries.GetOrders;
using ProToolRent.Domain.Entities;
using ProToolRent.Domain.Enums;
using ProToolRent.Domain.Interfaces;
using ProToolRent.Domain.ValueObjects;

namespace ProToolRent.Application.Tests;

public class GetOrdersQueryHandlerTests
{
    [Fact]
    public async Task Handle_WhenUserIsNull_ReturnsNotFound()
    {
        var mockUserRepo = new Mock<IUserRepository>();
        var mockOrderRepo = new Mock<IOrderRepository>();
        var mockToolRepo = new Mock<IToolRepository>();

        var userId = Guid.NewGuid();

        mockUserRepo.Setup(repo => repo.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User)null!);
        
        var handler = new GetOrdersQueryHandler(mockUserRepo.Object, mockOrderRepo.Object, mockToolRepo.Object);
        var query = new GetOrdersQuery(userId);

        var result = await handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorType.NotFound);
        result.Error.Should().Be("User not found");

        mockUserRepo.Verify(
            repo => repo.GetByIdAsync(userId, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenUserIsAdmin_ReturnsFailure()
    {
        var mockUserRepo = new Mock<IUserRepository>();
        var mockOrderRepo = new Mock<IOrderRepository>();
        var mockToolRepo = new Mock<IToolRepository>();
        
        var user = new User(email: "email@test.com", passwordHash: "passHash", role: UserRole.Admin);

        mockUserRepo.Setup(repo => repo.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        
        var handler = new GetOrdersQueryHandler(mockUserRepo.Object, mockOrderRepo.Object, mockToolRepo.Object);
        var query = new GetOrdersQuery(user.Id);

        var result = await handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorType.Failure);
        result.Error.Should().Be("Admin cant check orders");

        mockUserRepo.Verify(
            repo => repo.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenUserIsTenantWithZeroOrders_ReturnsSuccess()
    {
        var mockUserRepo = new Mock<IUserRepository>();
        var mockOrderRepo = new Mock<IOrderRepository>();
        var mockToolRepo = new Mock<IToolRepository>();
        var role = UserRole.Tenant;
        var user = new User(email: "email@test.com", passwordHash: "passHash", role: role);
        var orders = new List<Order>();

        mockUserRepo.Setup(repo => repo.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        mockOrderRepo.Setup(repo => repo.GetOrdersByTenantAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(orders);
        
        var handler = new GetOrdersQueryHandler(mockUserRepo.Object, mockOrderRepo.Object, mockToolRepo.Object);
        var query = new GetOrdersQuery(user.Id);

        var result = await handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Count.Should().Be(0);

        mockUserRepo.Verify(
            repo => repo.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()),
            Times.Once);
        
        mockOrderRepo.Verify(
            repo => repo.GetOrdersByTenantAsync(user.Id, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenUserIsTenantWithOrdersWithoutOrderItems_ReturnsFailure()
    {
        var mockUserRepo = new Mock<IUserRepository>();
        var mockOrderRepo = new Mock<IOrderRepository>();
        var mockToolRepo = new Mock<IToolRepository>();
        var role = UserRole.Tenant;
        var user = new User(email: "email@test.com", passwordHash: "passHash", role: role);
        var order = new Order(user.Id);
        var orders = new List<Order>(){ order };

        mockUserRepo.Setup(repo => repo.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        mockOrderRepo.Setup(repo => repo.GetOrdersByTenantAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(orders);
        
        var handler = new GetOrdersQueryHandler(mockUserRepo.Object, mockOrderRepo.Object, mockToolRepo.Object);
        var query = new GetOrdersQuery(user.Id);

        var result = await handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorType.Failure);
        result.Error.Should().Be("Order item not found");

        mockUserRepo.Verify(
            repo => repo.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()),
            Times.Once);
        
        mockOrderRepo.Verify(
            repo => repo.GetOrdersByTenantAsync(user.Id, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenUserIsTenantAndLandlordOfToolNotFound_ReturnsNotFound()
    {
        var mockUserRepo = new Mock<IUserRepository>();
        var mockOrderRepo = new Mock<IOrderRepository>();
        var mockToolRepo = new Mock<IToolRepository>();

        var role = UserRole.Tenant;
        var tenant = new User(email: "email@test.com", passwordHash: "passHash", role: role);
        var landlordId = Guid.NewGuid();

        var order = new Order(tenant.Id);

        var specification = new Specification("Brand", "Name", 100);
        var quantity = new Quantity(5);
        var tool = new Tool(specification, quantity, "desc", 1000, Guid.NewGuid(), landlordId);
        var createdDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
        var endDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7));
        var reservedQuantity = 1;

        order.AddItem(createdDate, endDate, reservedQuantity, tool);
        var orders = new List<Order>(){ order };
        
        mockUserRepo.Setup(repo => repo.GetByIdAsync(tenant.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tenant);
        mockOrderRepo.Setup(repo => repo.GetOrdersByTenantAsync(tenant.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(orders);
        mockUserRepo.Setup(repo => repo.GetByIdAsync(landlordId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User)null!);
        
        var handler = new GetOrdersQueryHandler(mockUserRepo.Object, mockOrderRepo.Object, mockToolRepo.Object);
        var query = new GetOrdersQuery(tenant.Id);

        var result = await handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorType.NotFound);
        result.Error.Should().Be("Landlord not found");

        mockUserRepo.Verify(
            repo => repo.GetByIdAsync(tenant.Id, It.IsAny<CancellationToken>()),
            Times.Once);
        
        mockOrderRepo.Verify(
            repo => repo.GetOrdersByTenantAsync(tenant.Id, It.IsAny<CancellationToken>()),
            Times.Once);

        mockUserRepo.Verify(
            repo => repo.GetByIdAsync(landlordId, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenUserIsTenant_ReturnsSuccess()
    {
        var mockUserRepo = new Mock<IUserRepository>();
        var mockOrderRepo = new Mock<IOrderRepository>();
        var mockToolRepo = new Mock<IToolRepository>();

        var tenant = new User(email: "email@test.com", passwordHash: "passHash", role: UserRole.Tenant);
        var tenantProfile = new UserProfile("tenant", "last", "city", "org", "phone");
        tenant.SetProfile(tenantProfile);
        var landlord = new User(email: "email2@test.com", passwordHash: "passHash", role: UserRole.Landlord);
        var landlordProfile = new UserProfile("landlord", "last", "city", "org", "phone");
        landlord.SetProfile(landlordProfile);

        var order = new Order(tenant.Id);

        var specification = new Specification("Brand", "Name", 100);
        var quantity = new Quantity(5);
        var tool = new Tool(specification, quantity, "desc", 1000, Guid.NewGuid(), landlord.Id);
        var createdDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
        var endDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7));
        var reservedQuantity = 1;

        order.AddItem(createdDate, endDate, reservedQuantity, tool);
        var orders = new List<Order>(){ order };
        
        mockUserRepo.Setup(repo => repo.GetByIdAsync(tenant.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tenant);
        mockOrderRepo.Setup(repo => repo.GetOrdersByTenantAsync(tenant.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(orders);
        mockUserRepo.Setup(repo => repo.GetByIdAsync(landlord.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(landlord);
        
        var handler = new GetOrdersQueryHandler(mockUserRepo.Object, mockOrderRepo.Object, mockToolRepo.Object);
        var query = new GetOrdersQuery(tenant.Id);

        var result = await handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNullOrEmpty();
        result.Value[0].OrderId.Should().Be(order.Id);
        result.Value[0].ToolId.Should().Be(tool.Id);
        result.Value[0].TenantName.Should().Be("tenant last");
        result.Value[0].LandlordName.Should().Be("landlord last");

        mockUserRepo.Verify(
            repo => repo.GetByIdAsync(tenant.Id, It.IsAny<CancellationToken>()),
            Times.Once);
        
        mockOrderRepo.Verify(
            repo => repo.GetOrdersByTenantAsync(tenant.Id, It.IsAny<CancellationToken>()),
            Times.Once);

        mockUserRepo.Verify(
            repo => repo.GetByIdAsync(landlord.Id, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenUserIsLandlordWithZeroOrders_ReturnsSuccess()
    {
        var mockUserRepo = new Mock<IUserRepository>();
        var mockOrderRepo = new Mock<IOrderRepository>();
        var mockToolRepo = new Mock<IToolRepository>();
        var role = UserRole.Landlord;
        var user = new User(email: "email@test.com", passwordHash: "passHash", role: role);
        var orders = new List<Order>();

        mockUserRepo.Setup(repo => repo.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        mockOrderRepo.Setup(repo => repo.GetOrdersByLandlordAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(orders);
        
        var handler = new GetOrdersQueryHandler(mockUserRepo.Object, mockOrderRepo.Object, mockToolRepo.Object);
        var query = new GetOrdersQuery(user.Id);

        var result = await handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Count.Should().Be(0);

        mockUserRepo.Verify(
            repo => repo.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()),
            Times.Once);
        
        mockOrderRepo.Verify(
            repo => repo.GetOrdersByLandlordAsync(user.Id, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenUserIsLandlordWithOrdersWithoutOrderItems_ReturnsFailure()
    {
        var mockUserRepo = new Mock<IUserRepository>();
        var mockOrderRepo = new Mock<IOrderRepository>();
        var mockToolRepo = new Mock<IToolRepository>();
        var role = UserRole.Landlord;
        var user = new User(email: "email@test.com", passwordHash: "passHash", role: role);
        var order = new Order(user.Id);
        var orders = new List<Order>(){ order };

        mockUserRepo.Setup(repo => repo.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        mockOrderRepo.Setup(repo => repo.GetOrdersByLandlordAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(orders);
        
        var handler = new GetOrdersQueryHandler(mockUserRepo.Object, mockOrderRepo.Object, mockToolRepo.Object);
        var query = new GetOrdersQuery(user.Id);

        var result = await handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorType.Failure);
        result.Error.Should().Be("Order item not found");

        mockUserRepo.Verify(
            repo => repo.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()),
            Times.Once);
        
        mockOrderRepo.Verify(
            repo => repo.GetOrdersByLandlordAsync(user.Id, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenUserIsLandlordAndTenantOfOrderNotFound_ReturnsNotFound()
    {
        var mockUserRepo = new Mock<IUserRepository>();
        var mockOrderRepo = new Mock<IOrderRepository>();
        var mockToolRepo = new Mock<IToolRepository>();

        var role = UserRole.Landlord;
        var landlord = new User(email: "email@test.com", passwordHash: "passHash", role: role);
        var tenantId = Guid.NewGuid();

        var order = new Order(tenantId);

        var specification = new Specification("Brand", "Name", 100);
        var quantity = new Quantity(5);
        var tool = new Tool(specification, quantity, "desc", 1000, Guid.NewGuid(), landlord.Id);
        var createdDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
        var endDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7));
        var reservedQuantity = 1;

        order.AddItem(createdDate, endDate, reservedQuantity, tool);
        var orders = new List<Order>(){ order };
        
        mockUserRepo.Setup(repo => repo.GetByIdAsync(landlord.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(landlord);
        mockOrderRepo.Setup(repo => repo.GetOrdersByLandlordAsync(landlord.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(orders);
        mockUserRepo.Setup(repo => repo.GetByIdAsync(tenantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User)null!);
        
        var handler = new GetOrdersQueryHandler(mockUserRepo.Object, mockOrderRepo.Object, mockToolRepo.Object);
        var query = new GetOrdersQuery(landlord.Id);

        var result = await handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorType.NotFound);
        result.Error.Should().Be("Tenant not found");

        mockUserRepo.Verify(
            repo => repo.GetByIdAsync(landlord.Id, It.IsAny<CancellationToken>()),
            Times.Once);
        
        mockOrderRepo.Verify(
            repo => repo.GetOrdersByLandlordAsync(landlord.Id, It.IsAny<CancellationToken>()),
            Times.Once);

        mockUserRepo.Verify(
            repo => repo.GetByIdAsync(tenantId, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenUserIsLandlord_ReturnsSuccess()
    {
        var mockUserRepo = new Mock<IUserRepository>();
        var mockOrderRepo = new Mock<IOrderRepository>();
        var mockToolRepo = new Mock<IToolRepository>();

        var tenant = new User(email: "email@test.com", passwordHash: "passHash", role: UserRole.Tenant);
        var tenantProfile = new UserProfile("tenant", "last", "city", "org", "phone");
        tenant.SetProfile(tenantProfile);
        var landlord = new User(email: "email2@test.com", passwordHash: "passHash", role: UserRole.Landlord);
        var landlordProfile = new UserProfile("landlord", "last", "city", "org", "phone");
        landlord.SetProfile(landlordProfile);

        var order = new Order(tenant.Id);

        var specification = new Specification("Brand", "Name", 100);
        var quantity = new Quantity(5);
        var tool = new Tool(specification, quantity, "desc", 1000, Guid.NewGuid(), landlord.Id);
        var createdDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
        var endDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7));
        var reservedQuantity = 1;

        order.AddItem(createdDate, endDate, reservedQuantity, tool);
        var orders = new List<Order>(){ order };
        
        mockUserRepo.Setup(repo => repo.GetByIdAsync(landlord.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(landlord);
        mockOrderRepo.Setup(repo => repo.GetOrdersByLandlordAsync(landlord.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(orders);
        mockUserRepo.Setup(repo => repo.GetByIdAsync(tenant.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tenant);
        
        var handler = new GetOrdersQueryHandler(mockUserRepo.Object, mockOrderRepo.Object, mockToolRepo.Object);
        var query = new GetOrdersQuery(landlord.Id);

        var result = await handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNullOrEmpty();
        result.Value[0].OrderId.Should().Be(order.Id);
        result.Value[0].ToolId.Should().Be(tool.Id);
        result.Value[0].TenantName.Should().Be("tenant last");
        result.Value[0].LandlordName.Should().Be("landlord last");

        mockUserRepo.Verify(
            repo => repo.GetByIdAsync(landlord.Id, It.IsAny<CancellationToken>()),
            Times.Once);
        
        mockOrderRepo.Verify(
            repo => repo.GetOrdersByLandlordAsync(landlord.Id, It.IsAny<CancellationToken>()),
            Times.Once);

        mockUserRepo.Verify(
            repo => repo.GetByIdAsync(tenant.Id, It.IsAny<CancellationToken>()),
            Times.Once);
    }
}

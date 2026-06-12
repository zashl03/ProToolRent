using FluentAssertions;
using Moq;
using ProToolRent.Application.Commands.CreateOrder;
using ProToolRent.Domain.Entities;
using ProToolRent.Domain.Enums;
using ProToolRent.Domain.Interfaces;
using ProToolRent.Domain.ValueObjects;

namespace ProToolRent.Application.Tests;

public class CreateOrderCommandHandlerTests
{
    private record Mocks(
        Mock<IOrderRepository> OrderRepo,
        Mock<IToolRepository> ToolRepo,
        Mock<IUserRepository> UserRepo,
        Mock<IUnitOfWork> UnitOfWork)
    {
        public CreateOrderCommandHandler CreateHandler()
            => new(OrderRepo.Object, ToolRepo.Object, UserRepo.Object, UnitOfWork.Object);
    }

    private static Mocks CreateMocks() => new(
        new Mock<IOrderRepository>(),
        new Mock<IToolRepository>(),
        new Mock<IUserRepository>(),
        new Mock<IUnitOfWork>()
    );

    private static Tool CreateValidTool(Guid userId, int availableQuantity = 10)
    {
        var specification = new Specification(brand: "Brand", name: "Name", power: 100);
        var quantity = new Quantity(availableQuantity);
        var tool = new Tool(
            specification: specification, 
            quantity: quantity,
            description: "desc",
            price: 500,
            categoryId: Guid.NewGuid(),
            userId);
        return tool;
    }

    private static User CreateValidUser(string firstName, string lastName, UserRole userRole)
    {
        var user = new User(email: "email@test.com", passwordHash: "passwordHash", role: userRole);
        var profile = UserProfile.CreateEmpty();
        profile.UpdateProfile(firstName, lastName, "City", "Org", "1234567890");
        user.SetProfile(profile);
        return user;
    }

    [Fact]
    public async Task Handle_WhenToolIsNull_ReturnsNotFound()
    {
        var mocks = CreateMocks();
        var userId = Guid.NewGuid();
        var toolId = Guid.NewGuid();
        var quantity = 1;
        var startDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
        var endDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7));     

        mocks.ToolRepo.Setup(repo => repo.GetByIdAsync(toolId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Tool)null!);

        var handler = mocks.CreateHandler();  
        var command = new CreateOrderCommand(userId, toolId, startDate, endDate, quantity);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(Common.ErrorType.NotFound);
        result.Error.Should().Be($"Tool with {toolId} not found");

        mocks.ToolRepo.Verify(
            repo => repo.GetByIdAsync(toolId, It.IsAny<CancellationToken>()),
            Times.Once);
        
        mocks.OrderRepo.Verify(
            repo => repo.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()),
            Times.Never);
        
        mocks.UnitOfWork.Verify(
            uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
        
        mocks.UserRepo.Verify(
            repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenQuantityMoreThanAvailable_ReturnsConflict()
    {
        var mocks = CreateMocks();
        var userId = Guid.NewGuid();
        var tool = CreateValidTool(userId, 10);
        var quantityReserve = 11;
        var startDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
        var endDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)); 

        mocks.ToolRepo.Setup(repo => repo.GetByIdAsync(tool.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tool);
        
        var handler = mocks.CreateHandler();
        var command = new CreateOrderCommand(userId, tool.Id, startDate, endDate, quantityReserve);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(Common.ErrorType.Conflict);
        result.Error.Should().Be("Available quantity less than requested");

        mocks.ToolRepo.Verify(
            repo => repo.GetByIdAsync(tool.Id, It.IsAny<CancellationToken>()),
            Times.Once);
        
        mocks.UserRepo.Verify(
            repo => repo.GetByIdAsync(It.IsAny<Guid>(),It.IsAny<CancellationToken>()),
            Times.Never);
        
        mocks.OrderRepo.Verify(
            repo => repo.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()),
            Times.Never);
        
        mocks.UnitOfWork.Verify(
            uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenTenantIsNull_ReturnsConfict()
    {
        var mocks = CreateMocks();
        var tenantId = Guid.NewGuid();
        var landlordId = Guid.NewGuid();
        var tool = CreateValidTool(landlordId, 10);
        var quantityReserve = 5;
        var startDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
        var endDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)); 

        mocks.ToolRepo.Setup(repo => repo.GetByIdAsync(tool.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tool);
        mocks.UserRepo.Setup(repo => repo.GetByIdAsync(tenantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User)null!);
        
        var handler = mocks.CreateHandler();
        var command = new CreateOrderCommand(tenantId, tool.Id, startDate, endDate, quantityReserve);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(Common.ErrorType.Conflict);
        result.Error.Should().Be("Tenant not found");

        mocks.ToolRepo.Verify(
            repo => repo.GetByIdAsync(tool.Id, It.IsAny<CancellationToken>()),
            Times.Once);
        
        mocks.UserRepo.Verify(
            repo => repo.GetByIdAsync(tenantId, It.IsAny<CancellationToken>()),
            Times.Once);
        
        mocks.UserRepo.Verify(
            repo => repo.GetByIdAsync(landlordId, It.IsAny<CancellationToken>()),
            Times.Never);
        
        mocks.OrderRepo.Verify(
            repo => repo.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()),
            Times.Never);
        
        mocks.UnitOfWork.Verify(
            uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenLandlordIsNull_ReturnsConfict()
    {
        var mocks = CreateMocks();
        var tenant = CreateValidUser("ivan", "petr", UserRole.Tenant);
        var landlordId = Guid.NewGuid();
        var tool = CreateValidTool(landlordId, 10);
        var quantityReserve = 5;
        var startDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
        var endDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)); 

        mocks.ToolRepo.Setup(repo => repo.GetByIdAsync(tool.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tool);
        mocks.UserRepo.Setup(repo => repo.GetByIdAsync(tenant.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tenant);
        mocks.UserRepo.Setup(repo => repo.GetByIdAsync(landlordId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User)null!);
        
        var handler = mocks.CreateHandler();
        var command = new CreateOrderCommand(tenant.Id, tool.Id, startDate, endDate, quantityReserve);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(Common.ErrorType.Conflict);
        result.Error.Should().Be("Landlord not found");

        mocks.ToolRepo.Verify(
            repo => repo.GetByIdAsync(tool.Id, It.IsAny<CancellationToken>()),
            Times.Once);
        
        mocks.UserRepo.Verify(
            repo => repo.GetByIdAsync(tenant.Id, It.IsAny<CancellationToken>()),
            Times.Once);
        
        mocks.UserRepo.Verify(
            repo => repo.GetByIdAsync(landlordId, It.IsAny<CancellationToken>()),
            Times.Once);
        
        mocks.OrderRepo.Verify(
            repo => repo.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()),
            Times.Never);
        
        mocks.UnitOfWork.Verify(
            uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenDataIsValid_CreatesOrderObjectAndReturnsSuccess()
    {
        var mocks = CreateMocks();
        var tenant = CreateValidUser("ivan", "petrov", UserRole.Tenant);
        var landlord = CreateValidUser("petr", "ivanov", UserRole.Landlord);
        var tool = CreateValidTool(landlord.Id, 10);
        var quantityReserve = 5;
        var startDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
        var endDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)); 

        mocks.ToolRepo.Setup(repo => repo.GetByIdAsync(tool.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tool);
        mocks.UserRepo.Setup(repo => repo.GetByIdAsync(tenant.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tenant);
        mocks.UserRepo.Setup(repo => repo.GetByIdAsync(landlord.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(landlord);
        
        var handler = mocks.CreateHandler();
        var command = new CreateOrderCommand(tenant.Id, tool.Id, startDate, endDate, quantityReserve);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.TenantName.Should().Be("ivan petrov");
        result.Value.LandlordName.Should().Be("petr ivanov");
        result.Value.ToolId.Should().Be(tool.Id);
        result.Value.ToolQuantity.Should().Be(quantityReserve);
        result.Value.ToolPricePerDay.Should().Be(500); 

        mocks.ToolRepo.Verify(
            repo => repo.GetByIdAsync(tool.Id, It.IsAny<CancellationToken>()),
            Times.Once);
        
        mocks.UserRepo.Verify(
            repo => repo.GetByIdAsync(tenant.Id, It.IsAny<CancellationToken>()),
            Times.Once);
        
        mocks.UserRepo.Verify(
            repo => repo.GetByIdAsync(landlord.Id, It.IsAny<CancellationToken>()),
            Times.Once);
        
        mocks.OrderRepo.Verify(
            repo => repo.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()),
            Times.Once);
        
        mocks.UnitOfWork.Verify(
            uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }
}

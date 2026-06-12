using FluentAssertions;
using Moq;
using ProToolRent.Application.Commands.CreateOrderItem;
using ProToolRent.Application.Common;
using ProToolRent.Domain.Entities;
using ProToolRent.Domain.Interfaces;
using ProToolRent.Domain.ValueObjects;

namespace ProToolRent.Application.Tests;

public class CreateOrderItemCommandHandlerTests
{
    private record Mocks(
        Mock<IOrderRepository> OrderRepo,
        Mock<IToolRepository> ToolRepo,
        Mock<IUnitOfWork> UnitOfWork)
    {
        public CreateOrderItemCommandHandler CreateHandler()
            => new(OrderRepo.Object, ToolRepo.Object, UnitOfWork.Object);
    }
    private static Mocks CreateMocks() => new(
        new Mock<IOrderRepository>(),
        new Mock<IToolRepository>(),
        new Mock<IUnitOfWork>()
    );

    [Fact]
    public async Task Handle_WhenOrderIsNull_ReturnsNotFound()
    {
        var mocks = CreateMocks();
        var orderId = Guid.NewGuid();
        var toolId = Guid.NewGuid();
        var startDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
        var endDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7));
        var quantity = 1;

        mocks.OrderRepo.Setup(repo => repo.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order)null!);

        var handler = mocks.CreateHandler();
        var command = new CreateOrderItemCommand(orderId, toolId, startDate, endDate, quantity);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorType.NotFound);
        result.Error.Should().Be($"Order with {orderId} not found");

        mocks.OrderRepo.Verify(
            repo => repo.GetByIdAsync(orderId, It.IsAny<CancellationToken>()),
            Times.Once);
        mocks.ToolRepo.Verify(
            repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never);
        mocks.UnitOfWork.Verify(
            uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenToolIsNull_ReturnsNotFound()
    {
        var mocks = CreateMocks();
        var order = new Order(Guid.NewGuid());
        var toolId = Guid.NewGuid();
        var startDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
        var endDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7));
        var quantity = 1;

        mocks.OrderRepo.Setup(repo => repo.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);
        mocks.ToolRepo.Setup(repo => repo.GetByIdAsync(toolId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Tool)null!);

        var handler = mocks.CreateHandler();
        var command = new CreateOrderItemCommand(order.Id, toolId, startDate, endDate, quantity);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorType.NotFound);
        result.Error.Should().Be($"Tool with {toolId} not found");

        mocks.OrderRepo.Verify(
            repo => repo.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()),
            Times.Once);
        mocks.ToolRepo.Verify(
            repo => repo.GetByIdAsync(toolId, It.IsAny<CancellationToken>()),
            Times.Once);
        mocks.UnitOfWork.Verify(
            uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenDataIsValid_CreatesOrderItemObjectAndReturnsSuccess()
    {
        var mocks = CreateMocks();
        var order = new Order(Guid.NewGuid());
        var specification = new Specification("Brand", "Name", 100);
        var quantity = new Quantity(5);
        var tool = new Tool(
            specification: specification, 
            quantity: quantity, 
            description: "desc",
            price: 500,
            categoryId: Guid.NewGuid(),
            userId: Guid.NewGuid());
        var startDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
        var endDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7));
        var quantityReserve = 1;

        mocks.OrderRepo.Setup(repo => repo.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);
        mocks.ToolRepo.Setup(repo => repo.GetByIdAsync(tool.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tool);

        var handler = mocks.CreateHandler();
        var command = new CreateOrderItemCommand(order.Id, tool.Id, startDate, endDate, quantityReserve);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
        result.Value.Should().Be(order.OrderItems.Single().Id);

        mocks.OrderRepo.Verify(
            repo => repo.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()),
            Times.Once);
        mocks.ToolRepo.Verify(
            repo => repo.GetByIdAsync(tool.Id, It.IsAny<CancellationToken>()),
            Times.Once);
        mocks.UnitOfWork.Verify(
            uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }
}

using FluentAssertions;
using Moq;
using ProToolRent.Application.Commands.DeleteOrderItem;
using ProToolRent.Application.Common;
using ProToolRent.Domain.Entities;
using ProToolRent.Domain.Interfaces;
using ProToolRent.Domain.ValueObjects;

namespace ProToolRent.Application.Tests;

public class DeleteOrderItemCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenOrderIsNull_ReturnsNotFound()
    {
        var mockOrderRepo = new Mock<IOrderRepository>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var orderId = Guid.NewGuid();
        var orderItemId = Guid.NewGuid();

        mockOrderRepo.Setup(repo => repo.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order)null!);

        var handler = new DeleteOrderItemCommandHandler(mockOrderRepo.Object, mockUnitOfWork.Object);
        var command = new DeleteOrderItemCommand(orderId, orderItemId);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorType.NotFound);
        result.Error.Should().Be($"Order with {orderId} not found");

        mockOrderRepo.Verify(
            repo => repo.GetByIdAsync(orderId, It.IsAny<CancellationToken>()), 
            Times.Once);
        
        mockUnitOfWork.Verify(
            uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), 
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenOrderItemIsNull_ReturnsNotFound()
    {
        var mockOrderRepo = new Mock<IOrderRepository>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var order = new Order(Guid.NewGuid());
        var orderItemId = Guid.NewGuid();

        mockOrderRepo.Setup(repo => repo.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        var handler = new DeleteOrderItemCommandHandler(mockOrderRepo.Object, mockUnitOfWork.Object);
        var command = new DeleteOrderItemCommand(order.Id, orderItemId);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorType.NotFound);
        result.Error.Should().Be($"Order item with {orderItemId} not found");

        mockOrderRepo.Verify(
            repo => repo.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()), 
            Times.Once);
        
        mockUnitOfWork.Verify(
            uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), 
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenDataIsValid_RemovesOrderItemAndReturnsSuccess()
    {
        var mockOrderRepo = new Mock<IOrderRepository>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
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
        var orderItemId = order.AddItem(
            createdDate: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            endDate: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)),
            quantity: 1,
            tool: tool);

        mockOrderRepo.Setup(repo => repo.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        var handler = new DeleteOrderItemCommandHandler(mockOrderRepo.Object, mockUnitOfWork.Object);
        var command = new DeleteOrderItemCommand(order.Id, orderItemId);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        mockOrderRepo.Verify(
            repo => repo.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()), 
            Times.Once);
        
        mockUnitOfWork.Verify(
            uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), 
            Times.Once);
    }
}

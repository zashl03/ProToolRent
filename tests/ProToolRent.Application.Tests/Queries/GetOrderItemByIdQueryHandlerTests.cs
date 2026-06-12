using FluentAssertions;
using Moq;
using ProToolRent.Application.Common;
using ProToolRent.Application.Queries.GetOrderItemById;
using ProToolRent.Domain.Entities;
using ProToolRent.Domain.Interfaces;
using ProToolRent.Domain.ValueObjects;

namespace ProToolRent.Application.Tests;

public class GetOrderItemByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_WhenOrderItemExists_ReturnsSuccessWithOrderItem()
    {
        var mockOrderRepo = new Mock<IOrderRepository>();
        var specification = new Specification("Brand", "Name", 100);
        var quantity = new Quantity(5);
        var tool = new Tool(specification, quantity, "desc", 1000, Guid.NewGuid(), Guid.NewGuid());
        var order = new Order(Guid.NewGuid());
        var createdDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
        var endDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7));
        order.AddItem(
            createdDate: createdDate,
            endDate: endDate,
            quantity: 1,
            tool: tool);
        var orderItemId = order.OrderItems.Single().Id;

        mockOrderRepo.Setup(repo => repo.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        var handler = new GetOrderItemByIdQueryHandler(mockOrderRepo.Object);
        var query = new GetOrderItemByIdQuery(order.Id, orderItemId);

        var result = await handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().Be(orderItemId);
        result.Value.CreatedDate.Should().Be(createdDate);
        result.Value.EndDate.Should().Be(endDate);
        result.Value.Cost.Should().Be((endDate.DayNumber - createdDate.DayNumber) * 1000);
        result.Value.Quantity.Should().Be(1);
        result.Value.ToolId.Should().Be(tool.Id);

        mockOrderRepo.Verify(
            repo => repo.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenOrderIsNull_ReturnsNotFound()
    {
        var mockOrderRepo = new Mock<IOrderRepository>();
        var orderId = Guid.NewGuid();
        var orderItemId = Guid.NewGuid();

        mockOrderRepo.Setup(repo => repo.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order)null!);

        var handler = new GetOrderItemByIdQueryHandler(mockOrderRepo.Object);
        var query = new GetOrderItemByIdQuery(orderId, orderItemId);

        var result = await handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorType.NotFound);
        result.Error.Should().Be($"Order with {orderId} not found");

        mockOrderRepo.Verify(
            repo => repo.GetByIdAsync(orderId, It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenOrderItemIsNull_ReturnsNotFound()
    {
        var mockOrderRepo = new Mock<IOrderRepository>();
        var order = new Order(Guid.NewGuid());
        var orderItemId = Guid.NewGuid();

        mockOrderRepo.Setup(repo => repo.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        var handler = new GetOrderItemByIdQueryHandler(mockOrderRepo.Object);
        var query = new GetOrderItemByIdQuery(order.Id, orderItemId);

        var result = await handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorType.NotFound);
        result.Error.Should().Be($"Order item with {orderItemId} not found");

        mockOrderRepo.Verify(
            repo => repo.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()), 
            Times.Once);
    }
}

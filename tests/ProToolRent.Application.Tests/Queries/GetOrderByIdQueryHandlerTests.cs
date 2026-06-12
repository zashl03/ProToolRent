using FluentAssertions;
using Moq;
using ProToolRent.Application.Common;
using ProToolRent.Application.Queries.GetOrderById;
using ProToolRent.Domain.Entities;
using ProToolRent.Domain.Interfaces;

namespace ProToolRent.Application.Tests;

public class GetOrderByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_WhenOrderExists_ReturnsSuccessWithOrder()
    {
        var mockOrderRepo = new Mock<IOrderRepository>();
        var order = new Order(Guid.NewGuid());

        mockOrderRepo.Setup(repo => repo.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        var handler = new GetOrderByIdQueryHandler(mockOrderRepo.Object);
        var query = new GetOrderByIdQuery(order.Id);

        var result = await handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().Be(order.Id);
        result.Value.Status.Should().Be("Создан");
        result.Value.CreatedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
        result.Value.OrderItems.Should().NotBeNull();

        mockOrderRepo.Verify(
            repo => repo.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenOrderIsNull_ReturnsNotFound()
    {
        var mockOrderRepo = new Mock<IOrderRepository>();
        var orderId = Guid.NewGuid();

        mockOrderRepo.Setup(repo => repo.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order)null!);

        var handler = new GetOrderByIdQueryHandler(mockOrderRepo.Object);
        var query = new GetOrderByIdQuery(orderId);

        var result = await handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorType.NotFound);
        result.Error.Should().Be($"Order with {orderId} not found");

        mockOrderRepo.Verify(
            repo => repo.GetByIdAsync(orderId, It.IsAny<CancellationToken>()), 
            Times.Once);
    }
}

using FluentAssertions;
using Moq;
using ProToolRent.Application.Common;
using ProToolRent.Application.Queries.GetToolsByUserId;
using ProToolRent.Domain.Entities;
using ProToolRent.Domain.Interfaces;
using ProToolRent.Domain.ValueObjects;

namespace ProToolRent.Application.Tests;

public class GetToolsByUserIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_WhenToolsIsExist_ReturnsSuccessWithTools()
    {
        var mockToolRepo = new Mock<IToolRepository>();

        var specification = new Specification("Brand", "Name", 100);
        var quantity = new Quantity(5);
        var userId = Guid.NewGuid();
        var tool = new Tool(specification, quantity, "desc", 1000, Guid.NewGuid(), userId);
        var tools = new List<Tool>(){tool};

        mockToolRepo.Setup(repo => repo.GetToolsByUserAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tools);
        
        var handler = new GetToolsByUserIdQueryHandler(mockToolRepo.Object);
        var query = new GetToolsByUserIdQuery(userId);

        var result = await handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().HaveCount(1);
        result.Value.Single().UserId.Should().Be(userId);

        mockToolRepo.Verify(repo => repo.GetToolsByUserAsync(userId, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenToolsIsEmpty_ReturnsSuccessWithEmptyList()
    {
        var mockToolRepo = new Mock<IToolRepository>();
        
        var userId = Guid.NewGuid();
        var tools = new List<Tool>();

        mockToolRepo.Setup(repo => repo.GetToolsByUserAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tools);
        
        var handler = new GetToolsByUserIdQueryHandler(mockToolRepo.Object);
        var query = new GetToolsByUserIdQuery(userId);

        var result = await handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorType.NotFound);
        result.Error.Should().Be($"User with {userId} haven tools");

        mockToolRepo.Verify(repo => repo.GetToolsByUserAsync(userId, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenToolsIsNull_ReturnsSuccessWithEmptyList()
    {
        var mockToolRepo = new Mock<IToolRepository>();
        
        var userId = Guid.NewGuid();

        mockToolRepo.Setup(repo => repo.GetToolsByUserAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((List<Tool>)null!);
        
        var handler = new GetToolsByUserIdQueryHandler(mockToolRepo.Object);
        var query = new GetToolsByUserIdQuery(userId);

        var result = await handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorType.NotFound);
        result.Error.Should().Be($"User with {userId} haven tools");

        mockToolRepo.Verify(repo => repo.GetToolsByUserAsync(userId, It.IsAny<CancellationToken>()),
            Times.Once);
    }
}

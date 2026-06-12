using FluentAssertions;
using Moq;
using ProToolRent.Domain.Entities;
using ProToolRent.Domain.Interfaces;
using ProToolRent.Domain.ValueObjects;

namespace ProToolRent.Application.Tests;

public class GetToolsQueryHandlerTests
{
    [Fact]
    public async Task Handle_WhenToolsIsExist_ReturnsSuccessWithTools()
    {
        var mockToolRepo = new Mock<IToolRepository>();

        var specification = new Specification("Brand", "Name", 100);
        var quantity = new Quantity(5);
        var userId = Guid.NewGuid();
        var tool = new Tool(specification, quantity, "desc", 1000, Guid.NewGuid(), userId);
        var tools = new PagedResult<Tool>(new List<Tool>(){tool}, 1);
        var page = 1;
        var size = 10;

        mockToolRepo.Setup(repo => repo.GetPagedAsync(userId, page, size, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tools);
        
        var handler = new GetToolsQueryHandler(mockToolRepo.Object);
        var query = new GetToolsQuery(userId, page, size);

        var result = await handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Items.Should().HaveCount(1);
        result.Value.Items.Single().UserId.Should().Be(userId);

        mockToolRepo.Verify(repo => repo.GetPagedAsync(userId, page, size, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenToolsIsEmpty_ReturnsSuccessWithEmptyList()
    {
        var mockToolRepo = new Mock<IToolRepository>();
        
        var userId = Guid.NewGuid();
        var tools = new PagedResult<Tool>(new List<Tool>(), 1);
        var page = 1;
        var size = 10;

        mockToolRepo.Setup(repo => repo.GetPagedAsync(userId, page, size, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tools);
        
        var handler = new GetToolsQueryHandler(mockToolRepo.Object);
        var query = new GetToolsQuery(userId, page, size);

        var result = await handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Items.Should().BeEmpty();

        mockToolRepo.Verify(repo => repo.GetPagedAsync(userId, page, size, It.IsAny<CancellationToken>()),
            Times.Once);
    }
}

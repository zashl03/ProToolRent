using FluentAssertions;
using Moq;
using ProToolRent.Application.Common;
using ProToolRent.Application.Queries.GetToolById;
using ProToolRent.Domain.Entities;
using ProToolRent.Domain.Interfaces;
using ProToolRent.Domain.ValueObjects;

namespace ProToolRent.Application.Tests;

public class GetToolByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_WhenUserIsNull_ReturnsNotFound()
    {
        var mockToolRepo = new Mock<IToolRepository>();
        var toolId = Guid.NewGuid();

        mockToolRepo.Setup(repo => repo.GetByIdAsync(toolId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Tool)null!);
        
        var handler = new GetToolByIdQueryHandler(mockToolRepo.Object);
        var query = new GetToolByIdQuery(toolId);

        var result = await handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorType.NotFound);
        result.Error.Should().Be($"Tool {toolId} not found");

        mockToolRepo.Verify(repo => repo.GetByIdAsync(toolId, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenDataIsValid_ReturnsNotFound()
    {
        var mockToolRepo = new Mock<IToolRepository>();
        var specification = new Specification("Brand", "Name", 100);
        var quantity = new Quantity(5);
        var tool = new Tool(specification, quantity, "desc", 1000, Guid.NewGuid(), Guid.NewGuid());

        mockToolRepo.Setup(repo => repo.GetByIdAsync(tool.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tool);
        
        var handler = new GetToolByIdQueryHandler(mockToolRepo.Object);
        var query = new GetToolByIdQuery(tool.Id);

        var result = await handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Description.Should().Be("desc");
        result.Value.Power.Should().Be(100);
        result.Value.Price.Should().Be(1000);
        result.Value.AvailableQuantity.Should().Be(5);

        mockToolRepo.Verify(repo => repo.GetByIdAsync(tool.Id, It.IsAny<CancellationToken>()),
            Times.Once);
    }
}

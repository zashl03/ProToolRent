using FluentAssertions;
using Moq;
using ProToolRent.Application.Commands.CreateTool;
using ProToolRent.Domain.Entities;
using ProToolRent.Domain.Interfaces;

namespace ProToolRent.Application.Tests;

public class CreateToolCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenDataIsValid_CreatesToolObjectAndReturnsSuccess()
    {
        var mockToolRepo = new Mock<IToolRepository>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();

        var handler = new CreateToolCommandHandler(mockToolRepo.Object, mockUnitOfWork.Object);
        var command = new CreateToolCommand(
            Brand: "brand",
            Name: "name",
            Power: 1000,
            Description: "desc",
            TotalQuantity: 10,
            Price: 500,
            CategoryId: Guid.NewGuid(),
            UserId: Guid.NewGuid());
        
        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        mockToolRepo.Verify(
            repo => repo.AddAsync(It.IsAny<Tool>(), It.IsAny<CancellationToken>()),
            Times.Once);
        mockUnitOfWork.Verify(
            uow => uow.SaveChangeAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }
}

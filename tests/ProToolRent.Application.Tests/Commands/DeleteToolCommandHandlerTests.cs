using FluentAssertions;
using Moq;
using ProToolRent.Application.Commands.DeleteTool;
using ProToolRent.Application.Common;
using ProToolRent.Domain.Entities;
using ProToolRent.Domain.Interfaces;
using ProToolRent.Domain.ValueObjects;

namespace ProToolRent.Application.Tests;

public class DeleteToolCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenToolIsNull_ReturnsNotFound()
    {
        var mockToolRepo = new Mock<IToolRepository>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var toolId = Guid.NewGuid();

        mockToolRepo.Setup(repo => repo.GetByIdAsync(toolId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Tool)null!);
        
        var handler = new DeleteToolCommandHandler(mockToolRepo.Object, mockUnitOfWork.Object);
        var command = new DeleteToolCommand(toolId);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorType.NotFound);
        result.Error.Should().Be($"Tool with ID {toolId} not found");

        mockToolRepo.Verify(
            repo => repo.GetByIdAsync(toolId, It.IsAny<CancellationToken>()),
            Times.Once);

        mockToolRepo.Verify(
            repo => repo.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never);

        mockUnitOfWork.Verify(
            uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenDataIsValid_RemovesToolAndReturnsSuccess()
    {
        var mockToolRepo = new Mock<IToolRepository>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var specification = new Specification("Brand", "Name", 100);
        var quantity = new Quantity(5);
        var tool = new Tool(
            specification: specification, 
            quantity: quantity, 
            description: "desc",
            price: 500,
            categoryId: Guid.NewGuid(),
            userId: Guid.NewGuid());

        mockToolRepo.Setup(repo => repo.GetByIdAsync(tool.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tool);
        
        var handler = new DeleteToolCommandHandler(mockToolRepo.Object, mockUnitOfWork.Object);
        var command = new DeleteToolCommand(tool.Id);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        mockToolRepo.Verify(
            repo => repo.GetByIdAsync(tool.Id, It.IsAny<CancellationToken>()),
            Times.Once);

        mockToolRepo.Verify(
            repo => repo.DeleteAsync(tool.Id, It.IsAny<CancellationToken>()),
            Times.Once);

        mockUnitOfWork.Verify(
            uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }
}

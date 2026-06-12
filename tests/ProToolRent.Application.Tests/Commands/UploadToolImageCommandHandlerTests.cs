using FluentAssertions;
using Moq;
using ProToolRent.Application.Common;
using ProToolRent.Domain.Entities;
using ProToolRent.Domain.Interfaces;
using ProToolRent.Domain.ValueObjects;

namespace ProToolRent.Application.Tests;

public class UploadToolImageCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenToolIsNull_ReturnsNotFound()
    {
        var mockToolRepo = new Mock<IToolRepository>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var toolId = Guid.NewGuid();
        var imageUrl = "C:/Images";

        mockToolRepo.Setup(repo => repo.GetByIdAsync(toolId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Tool)null!);
        
        var handler = new UploadToolImageCommandHandler(mockToolRepo.Object, mockUnitOfWork.Object);
        var command = new UploadToolImageCommand(toolId, imageUrl);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorType.NotFound);
        result.Error.Should().Be($"Tool not found");

        mockToolRepo.Verify(
            repo => repo.GetByIdAsync(toolId, It.IsAny<CancellationToken>()),
            Times.Once);

        mockUnitOfWork.Verify(
            uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenDataIsValid_UploadsToolImageAndReturnsSuccess()
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
        var imageUrl = "C:/Images";

        mockToolRepo.Setup(repo => repo.GetByIdAsync(tool.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tool);
        
        var handler = new UploadToolImageCommandHandler(mockToolRepo.Object, mockUnitOfWork.Object);
        var command = new UploadToolImageCommand(tool.Id, imageUrl);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        tool.ImageUrl.Should().Be(imageUrl);

        mockToolRepo.Verify(
            repo => repo.GetByIdAsync(tool.Id, It.IsAny<CancellationToken>()),
            Times.Once);

        mockUnitOfWork.Verify(
            uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }
}

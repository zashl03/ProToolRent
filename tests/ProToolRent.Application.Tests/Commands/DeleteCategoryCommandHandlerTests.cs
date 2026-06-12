using FluentAssertions;
using Moq;
using ProToolRent.Application.Commands.DeleteCategory;
using ProToolRent.Application.Common;
using ProToolRent.Domain.Entities;
using ProToolRent.Domain.Interfaces;

namespace ProToolRent.Application.Tests;

public class DeleteCategoryCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenCategoryIsNull_ReturnsNotFound()
    {
        var mockCategoryRepo = new Mock<ICategoryRepository>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var categoryId = Guid.NewGuid();

        mockCategoryRepo.Setup(repo => repo.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category)null!);

        var handler = new DeleteCategoryCommandHandler(mockCategoryRepo.Object, mockUnitOfWork.Object);
        var command = new DeleteCategoryCommand(categoryId);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorType.NotFound);
        result.Error.Should().Be($"Category with {categoryId} not found");

        mockCategoryRepo.Verify(
            repo => repo.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()), 
            Times.Once);

        mockCategoryRepo.Verify(
            repo => repo.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), 
            Times.Never);
        
        mockUnitOfWork.Verify(
            uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), 
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenDataIsValid_RemovesCategoryObjectAndReturnsSuccess()
    {
        var mockCategoryRepo = new Mock<ICategoryRepository>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var category = new Category("Main tools");

        mockCategoryRepo.Setup(repo => repo.GetByIdAsync(category.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        var handler = new DeleteCategoryCommandHandler(mockCategoryRepo.Object, mockUnitOfWork.Object);
        var command = new DeleteCategoryCommand(category.Id);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        mockCategoryRepo.Verify(
            repo => repo.GetByIdAsync(category.Id, It.IsAny<CancellationToken>()), 
            Times.Once);

        mockCategoryRepo.Verify(
            repo => repo.DeleteAsync(category.Id, It.IsAny<CancellationToken>()), 
            Times.Once);
        
        mockUnitOfWork.Verify(
            uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), 
            Times.Once);
    }
}

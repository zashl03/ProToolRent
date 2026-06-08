using FluentAssertions;
using Moq;
using ProToolRent.Application.Commands.CreateCategory;
using ProToolRent.Domain.Entities;
using ProToolRent.Domain.Interfaces;

namespace ProToolRent.Application.Tests;

public class CreateCategoryCommandHandlerTests
{
    private static CreateCategoryCommand CreateCommand(string name, Guid? parentId = null)
        => new CreateCategoryCommand(name, parentId);

    [Fact]
    public async Task Handle_WhenParentIdIsNull_CreatesRootCategoryAndReturnsSuccess()
    {
        var mockCategoryRepo = new Mock<ICategoryRepository>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();

        var handler = new CreateCategoryCommandHandler(mockCategoryRepo.Object, mockUnitOfWork.Object);
        var command = CreateCommand("Eletric tools", parentId: null);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        mockCategoryRepo.Verify(
            repo => repo.AddAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()),
            Times.Once);
        
        mockUnitOfWork.Verify(
            uow => uow.SaveChangeAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenParentCategoryIsNotNull_CreatesCategoryWithParentAndReturnsSuccess()
    {
        var mockCategoryRepo = new Mock<ICategoryRepository>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();

        var parent = new Category("Main tools");

        mockCategoryRepo.Setup(repo => repo.GetByIdAsync(parent.Id, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(parent);

        var handler = new CreateCategoryCommandHandler(mockCategoryRepo.Object, mockUnitOfWork.Object);
        var command = CreateCommand("Electric tools", parent.Id);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        mockCategoryRepo.Verify(
            repo => repo.AddAsync(
                It.Is<Category>(c =>
                    c.Name == "Electric tools" &&
                    c.ParentId == parent.Id &&
                    c.Parent == parent), 
                It.IsAny<CancellationToken>()),
            Times.Once);
        
        mockUnitOfWork.Verify(
            uow => uow.SaveChangeAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenParentCategoryIsNotExist_ReturnsFailure()
    {
        var mockCategoryRepo = new Mock<ICategoryRepository>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();

        var parentId = Guid.NewGuid();

        mockCategoryRepo
            .Setup(repo => repo.GetByIdAsync(parentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category)null!);

        var handler = new CreateCategoryCommandHandler(mockCategoryRepo.Object, mockUnitOfWork.Object);
        var command = CreateCommand("Electric tools", parentId);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(Common.ErrorType.Failure);
        result.Error.Should().Be("That categoryId does not exist");

        mockCategoryRepo.Verify(
            repo => repo.GetByIdAsync(parentId, It.IsAny<CancellationToken>()),
            Times.Once);
        mockCategoryRepo.Verify(
            repo => repo.AddAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()),
            Times.Never);
        mockUnitOfWork.Verify(
            uow => uow.SaveChangeAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }
}

using FluentAssertions;
using Moq;
using ProToolRent.Application.Common;
using ProToolRent.Application.Queries.GetCategoryById;
using ProToolRent.Domain.Entities;
using ProToolRent.Domain.Interfaces;

namespace ProToolRent.Application.Tests;

public class GetCategoryByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_WhenCategoryExists_ReturnsSuccessWithCategory()
    {
        var mockCategoryRepo = new Mock<ICategoryRepository>();

        var category = new Category("Main tools");

        mockCategoryRepo.Setup(repo => repo.GetByIdAsync(category.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        var handler = new GetCategoryByIdQueryHandler(mockCategoryRepo.Object);
        var query = new GetCategoryByIdQuery(category.Id);

        var result = await handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().Be(category.Id);
        result.Value.Name.Should().Be("Main tools");
        result.Value.ParentId.Should().BeNull();

        mockCategoryRepo.Verify(
            repo => repo.GetByIdAsync(category.Id, It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenCategoryIsNull_ReturnsNotFound()
    {
        var mockCategoryRepo = new Mock<ICategoryRepository>();
        var categoryId = Guid.NewGuid();

        mockCategoryRepo.Setup(repo => repo.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category)null!);

        var handler = new GetCategoryByIdQueryHandler(mockCategoryRepo.Object);
        var query = new GetCategoryByIdQuery(categoryId);

        var result = await handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorType.NotFound);
        result.Error.Should().Be($"Category with {categoryId} not found");

        mockCategoryRepo.Verify(
            repo => repo.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()), 
            Times.Once);
    }
}

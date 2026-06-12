using FluentAssertions;
using Moq;
using ProToolRent.Domain.Entities;
using ProToolRent.Domain.Interfaces;

namespace ProToolRent.Application.Tests;

public class GetCategoriesQueryHandlerTests
{
    [Fact]
    public async Task Handle_WhenDataIsValid_ReturnsSuccessWithListOfCategories()
    {
        var mockCategoryRepo = new Mock<ICategoryRepository>();

        var category1 = new Category("First tools");
        var category2 = new Category("Second tools");
        var categoryList = new List<Category>() {category1, category2};

        mockCategoryRepo.Setup(repo => repo.ListAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(categoryList);

        var handler = new GetCategoriesQueryHandler(mockCategoryRepo.Object);
        var query = new GetCategoriesQuery();

        var result = await handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNullOrEmpty();
        result.Value.Count.Should().Be(2);

        result.Value[0].Name.Should().Be("First tools");
        result.Value[1].Name.Should().Be("Second tools");

        mockCategoryRepo.Verify(
            repo => repo.ListAsync(It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenNoCategoriesExist_ReturnsSuccessWithEmptyList()
    {
        var mockCategoryRepo = new Mock<ICategoryRepository>();
        
        mockCategoryRepo
            .Setup(repo => repo.ListAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Category>());

        var handler = new GetCategoriesQueryHandler(mockCategoryRepo.Object);
        var query = new GetCategoriesQuery();

        var result = await handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().BeEmpty();

        mockCategoryRepo.Verify(
            repo => repo.ListAsync(It.IsAny<CancellationToken>()), 
            Times.Once);
    }
}

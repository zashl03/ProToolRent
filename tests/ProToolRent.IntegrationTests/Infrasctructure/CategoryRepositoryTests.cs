using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ProToolRent.Domain.Entities;
using ProToolRent.Infrastructure.Repositories;

namespace ProToolRent.IntegrationTests;

public class CategoryRepositoryTests: DatabaseTestBase
{
    [Fact]
    public async Task AddAsync_WhenCategoryIsValid_ShouldPersistToDatabase()
    {
        var repository = new CategoryRepository(DbContext);
        await ClearDatabaseAsync();
        var category = new Category("Tools");

        await repository.AddAsync(category, TestContext.Current.CancellationToken);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var persistedCategory = await DbContext.Categories
            .FirstOrDefaultAsync(c => c.Id == category.Id, TestContext.Current.CancellationToken);
            
        persistedCategory.Should().NotBeNull();
        persistedCategory.Name.Should().Be("Tools");
    }

    [Fact]
    public async Task GetByIdAsync_WhenCategoryExists_ShouldReturnIt()
    {
        var repository = new CategoryRepository(DbContext);
        await ClearDatabaseAsync();
        var category = new Category("Tools");

        DbContext.Categories.Add(category);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        DbContext.ChangeTracker.Clear();

        var result = await repository.GetByIdAsync(category.Id, TestContext.Current.CancellationToken);

        result.Should().NotBeNull();
        result.Id.Should().Be(category.Id);
        result.Name.Should().Be("Tools");
    }

    [Fact]
    public async Task GetByIdAsync_WhenCategoryDoesNotExists_ShouldReturnNull()
    {
        var repository = new CategoryRepository(DbContext);
        await ClearDatabaseAsync();
        var categoryId = Guid.NewGuid();

        var result = await repository.GetByIdAsync(categoryId, TestContext.Current.CancellationToken);

        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WhenCategoryExists_ShouldRemoveIt()
    {
        var repository = new CategoryRepository(DbContext);
        await ClearDatabaseAsync();
        var category = new Category("Tools");

        DbContext.Categories.Add(category);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        DbContext.ChangeTracker.Clear();

        await repository.DeleteAsync(category.Id, TestContext.Current.CancellationToken);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var removedCategory = await DbContext.Categories
            .FirstOrDefaultAsync(c => c.Id == category.Id, TestContext.Current.CancellationToken);
        
        removedCategory.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WhenCategoryDoesNotExists_ShouldNotThrow()
    {
        var repository = new CategoryRepository(DbContext);
        await ClearDatabaseAsync();
        var categoryId = Guid.NewGuid();

        await repository.DeleteAsync(categoryId, TestContext.Current.CancellationToken);

        var removedCategory = await DbContext.Categories
            .FirstOrDefaultAsync(c => c.Id == categoryId, TestContext.Current.CancellationToken);
        
        removedCategory.Should().BeNull();
    }

    [Fact]
    public async Task ListAsync_WhenCategoriesExist_ShouldReturnThem()
    {
        var repository = new CategoryRepository(DbContext);
        await ClearDatabaseAsync();
        var category1 = new Category("Electric tools");
        var category2 = new Category("Manual tools");
        var category3 = new Category("Wireless tools", category1.Id, category1);

        DbContext.Categories.Add(category1);
        DbContext.Categories.Add(category2);
        DbContext.Categories.Add(category3);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        DbContext.ChangeTracker.Clear();

        var result = await repository.ListAsync(TestContext.Current.CancellationToken);
        
        result.Should().NotBeNullOrEmpty();
        result.Count.Should().Be(3);
    }

    [Fact]
    public async Task ListAsync_WhenCategoriesIsEmpty_ShouldReturnEmptyList()
    {
        var repository = new CategoryRepository(DbContext);
        await ClearDatabaseAsync();

        var result = await repository.ListAsync(TestContext.Current.CancellationToken);
        
        result.Should().BeEmpty();
    }
}

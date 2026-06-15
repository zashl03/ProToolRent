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
        var category = new Category("Tools");

        await repository.AddAsync(category, Ct);
        await DbContext.SaveChangesAsync(Ct);

        var persistedCategory = await DbContext.Categories
            .FirstOrDefaultAsync(c => c.Id == category.Id, Ct);
            
        persistedCategory.Should().NotBeNull();
        persistedCategory.Name.Should().Be("Tools");
    }

    [Fact]
    public async Task GetByIdAsync_WhenCategoryExists_ShouldReturnIt()
    {
        var repository = new CategoryRepository(DbContext);
        var category = new Category("Tools");

        DbContext.Categories.Add(category);
        await DbContext.SaveChangesAsync(Ct);

        DbContext.ChangeTracker.Clear();

        var result = await repository.GetByIdAsync(category.Id, Ct);

        result.Should().NotBeNull();
        result.Id.Should().Be(category.Id);
        result.Name.Should().Be("Tools");
    }

    [Fact]
    public async Task GetByIdAsync_WhenCategoryDoesNotExists_ShouldReturnNull()
    {
        var repository = new CategoryRepository(DbContext);
        var categoryId = Guid.NewGuid();

        var result = await repository.GetByIdAsync(categoryId, Ct);

        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WhenCategoryExists_ShouldRemoveIt()
    {
        var repository = new CategoryRepository(DbContext);
        var category = new Category("Tools");

        DbContext.Categories.Add(category);
        await DbContext.SaveChangesAsync(Ct);

        DbContext.ChangeTracker.Clear();

        await repository.DeleteAsync(category.Id, Ct);
        await DbContext.SaveChangesAsync(Ct);

        var removedCategory = await DbContext.Categories
            .FirstOrDefaultAsync(c => c.Id == category.Id, Ct);
        
        removedCategory.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WhenCategoryDoesNotExists_ShouldNotThrow()
    {
        var repository = new CategoryRepository(DbContext);
        var categoryId = Guid.NewGuid();

        await repository.DeleteAsync(categoryId, Ct);

        var removedCategory = await DbContext.Categories
            .FirstOrDefaultAsync(c => c.Id == categoryId, Ct);
        
        removedCategory.Should().BeNull();
    }

    [Fact]
    public async Task ListAsync_WhenCategoriesExist_ShouldReturnThem()
    {
        var repository = new CategoryRepository(DbContext);
        var category1 = new Category("Electric tools");
        var category2 = new Category("Manual tools");
        var category3 = new Category("Wireless tools", category1.Id, category1);

        DbContext.Categories.Add(category1);
        DbContext.Categories.Add(category2);
        DbContext.Categories.Add(category3);
        await DbContext.SaveChangesAsync(Ct);

        DbContext.ChangeTracker.Clear();

        var result = await repository.ListAsync(Ct);
        
        result.Should().NotBeNullOrEmpty();
        result.Count.Should().Be(3);
    }

    [Fact]
    public async Task ListAsync_WhenCategoriesIsEmpty_ShouldReturnEmptyList()
    {
        var repository = new CategoryRepository(DbContext);

        var result = await repository.ListAsync(Ct);
        
        result.Should().BeEmpty();
    }
}

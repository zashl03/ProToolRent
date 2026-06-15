using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ProToolRent.Application.Common;
using ProToolRent.Domain.Entities;
using ProToolRent.Domain.Enums;
using ProToolRent.Domain.ValueObjects;
using ProToolRent.Infrastructure.Repositories;

namespace ProToolRent.IntegrationTests;

public class ToolRepositoryTests : DatabaseTestBase
{
    [Fact]
    public async Task GetByIdAsync_WhenToolExists_ShouldReturnsIt()
    {
        var repository = new ToolRepository(DbContext);
        var category = await CreateCategoryAsync();
        var user = await CreateUserAsync(role: UserRole.Landlord);
        var tool = await CreateToolAsync(category, user);

        var result = await repository.GetByIdAsync(tool.Id, Ct);

        result.Should().NotBeNull();
        result.CategoryId.Should().Be(category.Id);
        result.UserId.Should().Be(user.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenToolDoesNotExists_ShouldReturnsNull()
    {
        var repository = new ToolRepository(DbContext);
        var toolId = Guid.NewGuid();

        var result = await repository.GetByIdAsync(toolId, Ct);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetToolsByUserAsync_WhenToolAndUserExist_ShouldReturnList()
    {
        var repository = new ToolRepository(DbContext);
        var category = await CreateCategoryAsync();
        var user = await CreateUserAsync(role: UserRole.Landlord);
        var tool = await CreateToolAsync(category, user);

        var result = await repository.GetToolsByUserAsync(user.Id, Ct);

        result.Should().NotBeNull();
        result.Should().AllSatisfy(tool => 
            tool.CategoryId.Should().Be(category.Id));
        result.Should().AllSatisfy(tool => 
            tool.UserId.Should().Be(user.Id));
    }

    [Fact]
    public async Task AddAsync_WhenToolIsNotNull_ShouldPersistToDatabase()
    {
        var repository = new ToolRepository(DbContext);
        var category = await CreateCategoryAsync();
        var user = await CreateUserAsync(role: UserRole.Landlord);
        var spec = new Specification("brand", "name", 100);
        var quan = new Quantity(10);
        var tool = new Tool(spec, quan, "desc", 100, category.Id, user.Id);

        await repository.AddAsync(tool, Ct);
        await DbContext.SaveChangesAsync(Ct);

        DbContext.ChangeTracker.Clear();

        var persistedTool = await DbContext.Tools.FirstOrDefaultAsync(t => t.Id == tool.Id, Ct);
        
        persistedTool.Should().NotBeNull();
        persistedTool.Id.Should().Be(tool.Id);
        persistedTool.UserId.Should().Be(user.Id);
        persistedTool.CategoryId.Should().Be(category.Id);
    }

    [Fact]
    public async Task DeleteAsync_WhenToolIsNotNull_ShouldRemovesIt()
    {
        var repository = new ToolRepository(DbContext);
        var category = await CreateCategoryAsync();
        var user = await CreateUserAsync(role: UserRole.Landlord);
        var tool = await CreateToolAsync(category, user);

        await repository.DeleteAsync(tool.Id, Ct);
        await DbContext.SaveChangesAsync(Ct);

        var result = await DbContext.Tools.FirstOrDefaultAsync(t => t.Id == tool.Id, Ct);
        
        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WhenToolIsNull_ShouldNotThrow()
    {
        var repository = new ToolRepository(DbContext);
        var toolId = Guid.NewGuid();

        var act = async () => await repository.DeleteAsync(toolId, Ct);
        
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task GetPagedAsync_WhenAllTools_ShouldReturnList()
    {
        var repository = new ToolRepository(DbContext);
        var category = await CreateCategoryAsync();
        var user = await CreateUserAsync(role: UserRole.Landlord);
        var spec = new Specification("brand", "name", 100);
        var quan = new Quantity(10);
        var tool1 = new Tool(spec, quan, "desc", 100, category.Id, user.Id);
        var tool2 = new Tool(spec, quan, "desc2", 200, category.Id, user.Id);

        DbContext.Tools.Add(tool1);
        DbContext.Tools.Add(tool2);
        await DbContext.SaveChangesAsync(Ct);

        DbContext.ChangeTracker.Clear();

        var result = await repository.GetPagedAsync(2, 1, Ct);
        
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetPagedAsync_WhenUsersTools_ShouldReturnList()
    {
        var repository = new ToolRepository(DbContext);
        var category = await CreateCategoryAsync();
        var user1 = await CreateUserAsync(email: "landlord1@test.com", role: UserRole.Landlord);
        var user2 = await CreateUserAsync(email: "landlord2@test.com", role: UserRole.Landlord);
        var spec = new Specification("brand", "name", 100);
        var quan = new Quantity(10);
        var tool1 = new Tool(spec, quan, "desc", 100, category.Id, user1.Id);
        var tool2 = new Tool(spec, quan, "desc2", 200, category.Id, user2.Id);

        DbContext.Tools.Add(tool1);
        DbContext.Tools.Add(tool2);
        await DbContext.SaveChangesAsync(Ct);

        DbContext.ChangeTracker.Clear();

        var result = await repository.GetPagedAsync(user2.Id, 1, 10, Ct);
        
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(1);
        result.Items.Single().Id.Should().Be(tool2.Id);
    }

    [Fact]
    public async Task GetPagedAsync_WhenNoTools_ShouldReturnEmptyResult()
    {
        var repository = new ToolRepository(DbContext);
        
        var result = await repository.GetPagedAsync(1, 10, Ct);
        
        result.Should().NotBeNull();
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }

    [Fact]
    public async Task GetPagedAsync_WhenPageNumberTooLarge_ShouldReturnEmptyList()
    {
        var repository = new ToolRepository(DbContext);
        var category = await CreateCategoryAsync();
        var user = await CreateUserAsync(role: UserRole.Landlord);
        var tool = await CreateToolAsync(category, user);

        DbContext.ChangeTracker.Clear();

        var result = await repository.GetPagedAsync(100, 10, Ct);
        
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(1);
    }
    
    [Fact]
    public async Task GetToolsByUserAsync_WhenUserHasNoTools_ShouldReturnEmptyList()
    {
        var repository = new ToolRepository(DbContext);
        var user = await CreateUserAsync(role: UserRole.Landlord);

        var result = await repository.GetToolsByUserAsync(user.Id, Ct);

        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }
}

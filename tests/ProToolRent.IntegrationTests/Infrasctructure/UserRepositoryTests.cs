using FluentAssertions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using ProToolRent.Domain.Entities;
using ProToolRent.Domain.Enums;
using ProToolRent.Infrastructure.Repositories;

namespace ProToolRent.IntegrationTests;

public class UserRepositoryTests : DatabaseTestBase
{
    [Fact]
    public async Task GetByIdAsync_WhenUserIsNotNull_ShouldReturnsIt()
    {
        var repository = new UserRepository(DbContext);
        var user = await CreateUserAsync();

        var result = await repository.GetByIdAsync(user.Id, Ct);

        result.Should().NotBeNull();
        result.Email.Should().Be(user.Email);
        result.Id.Should().Be(user.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenUserIsNull_ShouldNull()
    {
        var repository = new UserRepository(DbContext);
        var userId = Guid.NewGuid();

        var result = await repository.GetByIdAsync(userId, Ct);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByEmailAsync_WhenUserIsNotNull_ShouldReturnsIt()
    {
        var repository = new UserRepository(DbContext);
        var user = await CreateUserAsync();

        var result = await repository.GetByEmailAsync(user.Email, Ct);

        result.Should().NotBeNull();
        result.Email.Should().Be(user.Email);
        result.Id.Should().Be(user.Id);
    }

    [Fact]
    public async Task GetByEmailAsync_WhenUserIsNull_ShouldNull()
    {
        var repository = new UserRepository(DbContext);
        var email = "test@test.com";

        var result = await repository.GetByEmailAsync(email, Ct);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByRefreshTokenAsync_WhenUserIsNotNull_ShouldReturnsIt()
    {
        var repository = new UserRepository(DbContext);
        var user = await CreateUserAsync();
        var refresh = "refreshToken!";
        var expires = DateTime.UtcNow.AddDays(7);
        user.SetRefreshToken(refresh, expires);

        DbContext.Users.Update(user);

        await DbContext.SaveChangesAsync(Ct);
        DbContext.ChangeTracker.Clear();

        var result = await repository.GetByRefreshTokenAsync(refresh, Ct);

        result.Should().NotBeNull();
        result.RefreshToken.Should().Be(refresh);
        result.Id.Should().Be(user.Id);
    }

    [Fact]
    public async Task GetByRefreshTokenAsync_WhenUserIsNull_ShouldNull()
    {
        var repository = new UserRepository(DbContext);
        var refresh = "refreshToken!";

        var result = await repository.GetByRefreshTokenAsync(refresh, Ct);

        result.Should().BeNull();
    }

    [Fact]
    public async Task AddAsync_WhenUserIsNotNull_ShouldPersistToDatabase()
    {
        var repository = new UserRepository(DbContext);
        var email = "test@test.com";
        var passHash = "passHash!";
        var user = new User(email, passHash, UserRole.Tenant);
        var profile = UserProfile.CreateEmpty();
        user.SetProfile(profile);

        await repository.AddAsync(user, Ct);
        await DbContext.SaveChangesAsync(Ct);

        DbContext.ChangeTracker.Clear();

        var persistedUser = await DbContext.Users
            .Include(u => u.Profile)
            .FirstOrDefaultAsync(u => u.Id == user.Id, Ct);

        persistedUser.Should().NotBeNull();
        persistedUser.Id.Should().Be(user.Id);
        persistedUser.Profile.Should().NotBeNull();
    }

    [Fact]
    public async Task DeleteAsync_WhenUserIsNotNull_ShouldRemovesIt()
    {
        var repository = new UserRepository(DbContext);
        var user = await CreateUserAsync();

        await repository.DeleteAsync(user.Id, Ct);
        await DbContext.SaveChangesAsync(Ct);

        DbContext.ChangeTracker.Clear();

        var result = await DbContext.Users.FirstOrDefaultAsync(t => t.Id == user.Id, Ct);

        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WhenUserIsNull_ShouldNotThrow()
    {
        var repository = new UserRepository(DbContext);
        var userId = Guid.NewGuid();

        var act = async () => await repository.DeleteAsync(userId, Ct);
        
        await act.Should().NotThrowAsync();
    }
}

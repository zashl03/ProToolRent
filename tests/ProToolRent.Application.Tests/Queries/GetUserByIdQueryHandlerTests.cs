using FluentAssertions;
using Moq;
using ProToolRent.Application.Common;
using ProToolRent.Application.Queries.GetUserById;
using ProToolRent.Domain.Entities;
using ProToolRent.Domain.Enums;
using ProToolRent.Domain.Interfaces;

namespace ProToolRent.Application.Tests;

public class GetUserByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_WhenUserIsNull_ReturnsNotFound()
    {
        var mockUserRepo = new Mock<IUserRepository>();
        var userId = Guid.NewGuid();

        mockUserRepo.Setup(repo => repo.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User)null!);
        
        var handler = new GetUserByIdQueryHandler(mockUserRepo.Object);
        var query = new GetUserByIdQuery(userId);

        var result = await handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorType.NotFound);
        result.Error.Should().Be($"User with {userId} not found");

        mockUserRepo.Verify(repo => repo.GetByIdAsync(userId, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenDataIsValid_ReturnsNotFound()
    {
        var mockUserRepo = new Mock<IUserRepository>();
        var email = "test@test.com";
        var pass = "passHash";
        var user = new User(email, pass, UserRole.Tenant);
        var profile = new UserProfile(
            firstName: "name",
            lastName: "last",
            city: "city",
            organization: "org",
            phone: "123456");
        user.SetProfile(profile);

        mockUserRepo.Setup(repo => repo.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        
        var handler = new GetUserByIdQueryHandler(mockUserRepo.Object);
        var query = new GetUserByIdQuery(user.Id);

        var result = await handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.FirstName.Should().Be("name");
        result.Value.LastName.Should().Be("last");
        result.Value.City.Should().Be("city");
        result.Value.Organization.Should().Be("org");
        result.Value.Phone.Should().Be("123456");
        result.Value.Role.Should().Be("Tenant");

        mockUserRepo.Verify(repo => repo.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()),
            Times.Once);
    }
}

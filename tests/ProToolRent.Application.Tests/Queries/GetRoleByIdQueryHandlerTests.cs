using System.Runtime.CompilerServices;
using FluentAssertions;
using Moq;
using ProToolRent.Application.Common;
using ProToolRent.Application.Queries.GetRoleById;
using ProToolRent.Domain.Entities;
using ProToolRent.Domain.Enums;
using ProToolRent.Domain.Interfaces;

namespace ProToolRent.Application.Tests;

public class GetRoleByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_WhenUserIsNull_ReturnsNotFound()
    {
        var mockUserRepo = new Mock<IUserRepository>();
        var userId = Guid.NewGuid();

        mockUserRepo.Setup(repo => repo.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User)null!);
        
        var handler = new GetRoleByIdQueryHandler(mockUserRepo.Object);
        var query = new GetRoleByIdQuery(userId);

        var result = await handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorType.NotFound);
        result.Error.Should().Be($"Role of user with {userId} not found");

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

        mockUserRepo.Setup(repo => repo.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        
        var handler = new GetRoleByIdQueryHandler(mockUserRepo.Object);
        var query = new GetRoleByIdQuery(user.Id);

        var result = await handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNullOrEmpty();
        result.Value.Should().Be("Tenant");

        mockUserRepo.Verify(repo => repo.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()),
            Times.Once);
    }
}

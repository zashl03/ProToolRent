using FluentAssertions;
using Moq;
using ProToolRent.Application.Authentication.Commands.Logout;
using ProToolRent.Application.Common;
using ProToolRent.Domain.Entities;
using ProToolRent.Domain.Enums;
using ProToolRent.Domain.Interfaces;

namespace ProToolRent.Application.Tests;

public class LogoutCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenUserIsNull_ReturnsNotFound()
    {
        var mockUserRepo = new Mock<IUserRepository>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var refresh = "refreshToken!";    

        mockUserRepo.Setup(repo => repo.GetByRefreshTokenAsync(refresh, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User)null!);

        var handler = new LogoutCommandHandler(mockUserRepo.Object,mockUnitOfWork.Object);
        var command = new LogoutCommand(refresh);
        
        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorType.NotFound);
        result.Error.Should().Be("User not found");

        mockUserRepo.Verify(
            repo => repo.GetByRefreshTokenAsync(refresh, It.IsAny<CancellationToken>()),
            Times.Once);
        
        mockUnitOfWork.Verify(
            uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenDataIsValid_LogoutsUserAndReturnsSuccess()
    {
        var mockUserRepo = new Mock<IUserRepository>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var user = new User("test@test.com", "password", UserRole.Tenant);
        var refresh = "refreshToken!";
        user.SetRefreshToken(refresh, DateTime.UtcNow.AddDays(7));    

        mockUserRepo.Setup(repo => repo.GetByRefreshTokenAsync(refresh, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var handler = new LogoutCommandHandler(mockUserRepo.Object,mockUnitOfWork.Object);
        var command = new LogoutCommand(refresh);
        
        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        user.RefreshToken.Should().BeNull();
        user.RefreshTokenExpiresAt.Should().BeNull();

        mockUserRepo.Verify(
            repo => repo.GetByRefreshTokenAsync(refresh, It.IsAny<CancellationToken>()),
            Times.Once);
        
        mockUnitOfWork.Verify(
            uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }
}

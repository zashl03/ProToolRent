using FluentAssertions;
using Moq;
using ProToolRent.Application.Authentication.Commands.Refresh;
using ProToolRent.Application.Common;
using ProToolRent.Application.Interfaces;
using ProToolRent.Domain.Entities;
using ProToolRent.Domain.Enums;
using ProToolRent.Domain.Interfaces;

namespace ProToolRent.Application.Tests;

public class RefreshCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenUserIsNull_ReturnsFailure()
    {
        var mockJwtProv = new Mock<IJwtProvider>();
        var mockUserRepo = new Mock<IUserRepository>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var refresh = "refreshToken!";    

        mockUserRepo.Setup(repo => repo.GetByRefreshTokenAsync(refresh, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User)null!);

        var handler = new RefreshCommandHandler(mockJwtProv.Object, mockUserRepo.Object, mockUnitOfWork.Object);
        var command = new RefreshCommand(refresh);
        
        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorType.Failure);
        result.Error.Should().Be("Invalid refresh token");

        mockUserRepo.Verify(
            repo => repo.GetByRefreshTokenAsync(refresh, It.IsAny<CancellationToken>()),
            Times.Once);

        mockJwtProv.Verify(
            jwt => jwt.GenerateAccessToken(It.IsAny<User>()),
            Times.Never);
        
        mockJwtProv.Verify(
            jwt => jwt.GenerateRefreshToken(),
            Times.Never);
        
        mockUnitOfWork.Verify(
            uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenRefreshExpires_ReturnsFailure()
    {
        var mockJwtProv = new Mock<IJwtProvider>();
        var mockUserRepo = new Mock<IUserRepository>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var user = new User("test@test.com", "password", UserRole.Tenant);
        var refresh = "refreshToken!";

        user.SetRefreshToken(refresh, DateTime.UtcNow.AddDays(7));
        var expiresAtProperty = typeof(User).GetProperty("RefreshTokenExpiresAt");
        expiresAtProperty!.SetValue(user, DateTime.UtcNow.AddDays(-1)); 

        mockUserRepo.Setup(repo => repo.GetByRefreshTokenAsync(refresh, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var handler = new RefreshCommandHandler(mockJwtProv.Object, mockUserRepo.Object, mockUnitOfWork.Object);
        var command = new RefreshCommand(refresh);
        
        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorType.Failure);
        result.Error.Should().Be("Invalid refresh token");

        mockUserRepo.Verify(
            repo => repo.GetByRefreshTokenAsync(refresh, It.IsAny<CancellationToken>()),
            Times.Once);

        mockJwtProv.Verify(
            jwt => jwt.GenerateAccessToken(It.IsAny<User>()),
            Times.Never);
        
        mockJwtProv.Verify(
            jwt => jwt.GenerateRefreshToken(),
            Times.Never);
        
        mockUnitOfWork.Verify(
            uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenDataIsValid_ReturnsSuccessWithTokenResponse()
    {
        var mockJwtProv = new Mock<IJwtProvider>();
        var mockUserRepo = new Mock<IUserRepository>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var user = new User("test@test.com", "password", UserRole.Tenant);
        var oldRefresh = "oldRefreshToken!";
        var newRefresh = "newRefreshToken!";
        var access = "accessToken!"; 
        user.SetRefreshToken(oldRefresh, DateTime.UtcNow.AddDays(7));

        mockUserRepo.Setup(repo => repo.GetByRefreshTokenAsync(oldRefresh, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        mockJwtProv.Setup(jwt => jwt.GenerateAccessToken(user))
            .Returns(access);
        mockJwtProv.Setup(jwt => jwt.GenerateRefreshToken())
            .Returns(newRefresh);

        var handler = new RefreshCommandHandler(mockJwtProv.Object, mockUserRepo.Object, mockUnitOfWork.Object);
        var command = new RefreshCommand(oldRefresh);
        
        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.AccessToken.Should().Be(access);
        result.Value.RefreshToken.Should().Be(newRefresh);
        user.RefreshToken.Should().Be(newRefresh);
        user.RefreshTokenExpiresAt.Should().BeCloseTo(DateTime.UtcNow.AddDays(7), TimeSpan.FromMinutes(1));

        mockUserRepo.Verify(
            repo => repo.GetByRefreshTokenAsync(oldRefresh, It.IsAny<CancellationToken>()),
            Times.Once);

        mockJwtProv.Verify(
            jwt => jwt.GenerateAccessToken(user),
            Times.Once);
        
        mockJwtProv.Verify(
            jwt => jwt.GenerateRefreshToken(),
            Times.Once);
        
        mockUnitOfWork.Verify(
            uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }
}

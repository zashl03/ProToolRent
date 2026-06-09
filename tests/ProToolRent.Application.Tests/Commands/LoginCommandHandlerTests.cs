using FluentAssertions;
using Moq;
using ProToolRent.Application.Authentication.Commands.Login;
using ProToolRent.Application.Common;
using ProToolRent.Application.Interfaces;
using ProToolRent.Domain.Entities;
using ProToolRent.Domain.Enums;
using ProToolRent.Domain.Interfaces;

namespace ProToolRent.Application.Tests;

public class LoginCommandHandlerTests
{
    private record Mocks(
        Mock<IJwtProvider> JwtProv,
        Mock<IPasswordHasher> PasswordHash,
        Mock<IUserRepository> UserRepo,
        Mock<IUnitOfWork> UnitOfWork)
    {
        public LoginCommandHandler CreateHandler()
            => new(JwtProv.Object, PasswordHash.Object, UserRepo.Object, UnitOfWork.Object);
    }

    private static Mocks CreateMocks() => new(
        new Mock<IJwtProvider>(),
        new Mock<IPasswordHasher>(),
        new Mock<IUserRepository>(),
        new Mock<IUnitOfWork>()
    );

    [Fact]
    public async Task Handle_WhenUserIsNull_ReturnsNotFound()
    {
        var mocks = CreateMocks();
        var userEmail = "test@test.com"; 
        var userPass = "password";   

        mocks.UserRepo.Setup(repo => repo.GetByEmailAsync(userEmail, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User)null!);

        var handler = mocks.CreateHandler();  
        var command = new LoginCommand(userEmail, userPass);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorType.NotFound);
        result.Error.Should().Be("Invalid input data");

        mocks.UserRepo.Verify(
            repo => repo.GetByEmailAsync(userEmail, It.IsAny<CancellationToken>()),
            Times.Once);
        
        mocks.PasswordHash.Verify(
            ph => ph.Verify(It.IsAny<string>(), It.IsAny<string>()),
            Times.Never);

        mocks.JwtProv.Verify(
            jwt => jwt.GenerateAccessToken(It.IsAny<User>()),
            Times.Never);
        
        mocks.JwtProv.Verify(
            jwt => jwt.GenerateRefreshToken(),
            Times.Never);
        
        mocks.UnitOfWork.Verify(
            uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenPasswordIsWrong_ReturnsFailure()
    {
        var mocks = CreateMocks();
        var userEmail = "test@test.com"; 
        var userPass = "password";
        var user = new User(userEmail, userPass, UserRole.Tenant);

        mocks.UserRepo.Setup(repo => repo.GetByEmailAsync(userEmail, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        mocks.PasswordHash.Setup(ph => ph.Verify("wrongPassword", user.PasswordHash))
            .Returns(false);

        var handler = mocks.CreateHandler();  
        var command = new LoginCommand(userEmail, Password: "wrongPassword");

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorType.Failure);
        result.Error.Should().Be("Invalid input data");

        mocks.UserRepo.Verify(
            repo => repo.GetByEmailAsync(userEmail, It.IsAny<CancellationToken>()),
            Times.Once);
        
        mocks.PasswordHash.Verify(
            ph => ph.Verify("wrongPassword", userPass),
            Times.Once);

        mocks.JwtProv.Verify(
            jwt => jwt.GenerateAccessToken(It.IsAny<User>()),
            Times.Never);
        
        mocks.JwtProv.Verify(
            jwt => jwt.GenerateRefreshToken(),
            Times.Never);
        
        mocks.UnitOfWork.Verify(
            uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenDataIsValid_ReturnsSuccessWithTokens()
    {
        var mocks = CreateMocks();
        var userEmail = "test@test.com"; 
        var userPass = "password";
        var user = new User(userEmail, userPass, UserRole.Tenant);
        var access = "accessToken!";
        var refresh = "refreshToken!";

        mocks.UserRepo.Setup(repo => repo.GetByEmailAsync(userEmail, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        mocks.PasswordHash.Setup(ph => ph.Verify(userPass, user.PasswordHash))
            .Returns(true);
        mocks.JwtProv.Setup(jwt => jwt.GenerateAccessToken(user))
            .Returns(access);
        mocks.JwtProv.Setup(jwt => jwt.GenerateRefreshToken())
            .Returns(refresh);

        var handler = mocks.CreateHandler();  
        var command = new LoginCommand(userEmail, userPass);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.UserId.Should().Be(user.Id);
        result.Value.AccessToken.Should().Be(access);
        result.Value.RefreshToken.Should().Be(refresh);
        result.Value.Role.Should().Be("Tenant");

        user.RefreshToken.Should().Be(refresh);
        user.RefreshTokenExpiresAt.Should().BeCloseTo(DateTime.UtcNow.AddDays(7), TimeSpan.FromMinutes(1));

        mocks.UserRepo.Verify(
            repo => repo.GetByEmailAsync(userEmail, It.IsAny<CancellationToken>()),
            Times.Once);
        
        mocks.PasswordHash.Verify(
            ph => ph.Verify(userPass, user.PasswordHash),
            Times.Once);

        mocks.JwtProv.Verify(
            jwt => jwt.GenerateAccessToken(It.IsAny<User>()),
            Times.Once);
        
        mocks.JwtProv.Verify(
            jwt => jwt.GenerateRefreshToken(),
            Times.Once);
        
        mocks.UnitOfWork.Verify(
            uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }
}

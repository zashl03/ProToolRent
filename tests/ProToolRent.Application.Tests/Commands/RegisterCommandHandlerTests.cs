using FluentAssertions;
using Moq;
using ProToolRent.Application.Authentication.Commands.Register;
using ProToolRent.Application.Common;
using ProToolRent.Application.Interfaces;
using ProToolRent.Domain.Entities;
using ProToolRent.Domain.Enums;
using ProToolRent.Domain.Interfaces;

namespace ProToolRent.Application.Tests;

public class RegisterCommandHandlerTests
{
    private record Mocks(
        Mock<IUserRepository> UserRepo,
        Mock<IPasswordHasher> PasswordHash,
        Mock<IJwtProvider> JwtProv,
        Mock<IUnitOfWork> UnitOfWork)
    {
        public RegisterCommandHandler CreateHandler()
            => new(UserRepo.Object, PasswordHash.Object, JwtProv.Object, UnitOfWork.Object);
    }

    private static Mocks CreateMocks() => new(
        new Mock<IUserRepository>(),
        new Mock<IPasswordHasher>(),
        new Mock<IJwtProvider>(),
        new Mock<IUnitOfWork>()
    );

    [Fact]
    public async Task Handle_WhenPasswordIsNotMatch_ReturnsConflict()
    {
        var mocks = CreateMocks();
        var userEmail = "test@test.com"; 
        var userPass = "password";
        var repeatPass = "wrongpassword";
        var role = "Tenant";   

        var handler = mocks.CreateHandler();  
        var command = new RegisterCommand(userEmail, userPass, repeatPass, role);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorType.Conflict);
        result.Error.Should().Be("Passwords do not match");

        mocks.UserRepo.Verify(
            repo => repo.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
        
        mocks.PasswordHash.Verify(
            ph => ph.Generate(It.IsAny<string>()),
            Times.Never);

        mocks.JwtProv.Verify(
            jwt => jwt.GenerateRefreshToken(),
            Times.Never);
        
        mocks.UserRepo.Verify(
            repo => repo.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()),
            Times.Never);
        
        mocks.UnitOfWork.Verify(
            uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
        
        mocks.JwtProv.Verify(
            jwt => jwt.GenerateAccessToken(It.IsAny<User>()),
            Times.Never); 
    }

    [Fact]
    public async Task Handle_WhenUserIsExists_ReturnsConflict()
    {
        var mocks = CreateMocks();
        var userEmail = "test@test.com"; 
        var userPass = "password";
        var repeatPass = "password";
        var role = "Tenant";   
        var user = new User(userEmail, userPass, UserRole.Tenant);

        mocks.UserRepo.Setup(repo => repo.GetByEmailAsync(userEmail, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var handler = mocks.CreateHandler();  
        var command = new RegisterCommand(userEmail, userPass, repeatPass, role);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorType.Conflict);
        result.Error.Should().Be($"User with {userEmail} exists");

        mocks.UserRepo.Verify(
            repo => repo.GetByEmailAsync(userEmail, It.IsAny<CancellationToken>()),
            Times.Once);
        
        mocks.PasswordHash.Verify(
            ph => ph.Generate(It.IsAny<string>()),
            Times.Never);

        mocks.JwtProv.Verify(
            jwt => jwt.GenerateRefreshToken(),
            Times.Never);
        
        mocks.UserRepo.Verify(
            repo => repo.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()),
            Times.Never);
        
        mocks.UnitOfWork.Verify(
            uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
        
        mocks.JwtProv.Verify(
            jwt => jwt.GenerateAccessToken(It.IsAny<User>()),
            Times.Never); 
    }

    [Fact]
    public async Task Handle_WhenRoleIsNotExists_ReturnsNotFound()
    {
        var mocks = CreateMocks();
        var userEmail = "test@test.com"; 
        var userPass = "password";
        var repeatPass = "password";
        var role = "Saler";   

        mocks.UserRepo.Setup(repo => repo.GetByEmailAsync(userEmail, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User)null!);

        var handler = mocks.CreateHandler();  
        var command = new RegisterCommand(userEmail, userPass, repeatPass, role);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorType.NotFound);
        result.Error.Should().Be("Role not found");

        mocks.UserRepo.Verify(
            repo => repo.GetByEmailAsync(userEmail, It.IsAny<CancellationToken>()),
            Times.Once);
        
        mocks.PasswordHash.Verify(
            ph => ph.Generate(It.IsAny<string>()),
            Times.Never);

        mocks.JwtProv.Verify(
            jwt => jwt.GenerateRefreshToken(),
            Times.Never);
        
        mocks.UserRepo.Verify(
            repo => repo.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()),
            Times.Never);
        
        mocks.UnitOfWork.Verify(
            uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
        
        mocks.JwtProv.Verify(
            jwt => jwt.GenerateAccessToken(It.IsAny<User>()),
            Times.Never); 
    }

    [Fact]
    public async Task Handle_WhenDataIsValid_ReturnsSuccessWithAuthResponse()
    {
        var mocks = CreateMocks();
        var userEmail = "test@test.com"; 
        var userPass = "password";
        var repeatPass = "password";
        var passwordHash = "passwordHash!";
        var role = "Tenant"; 
        var access = "accessToken!";
        var refresh = "refreshToken!";  

        mocks.UserRepo.Setup(repo => repo.GetByEmailAsync(userEmail, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User)null!);
        mocks.PasswordHash.Setup(ph => ph.Generate(userPass))
            .Returns(passwordHash);
        mocks.JwtProv.Setup(jwt => jwt.GenerateRefreshToken())
            .Returns(refresh);
        mocks.JwtProv.Setup(jwt => jwt.GenerateAccessToken(It.IsAny<User>()))
            .Returns(access);        

        var handler = mocks.CreateHandler();  
        var command = new RegisterCommand(userEmail, userPass, repeatPass, role);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.UserId.Should().NotBeEmpty();
        result.Value.AccessToken.Should().Be(access);
        result.Value.RefreshToken.Should().Be(refresh);
        result.Value.Role.Should().Be(role);

        mocks.UserRepo.Verify(
            repo => repo.GetByEmailAsync(userEmail, It.IsAny<CancellationToken>()),
            Times.Once);
        
        mocks.PasswordHash.Verify(
            ph => ph.Generate(userPass),
            Times.Once);

        mocks.JwtProv.Verify(
            jwt => jwt.GenerateRefreshToken(),
            Times.Once);
        
        mocks.UserRepo.Verify(
            repo => repo.AddAsync(
                It.Is<User>(u => 
                    u.Email == userEmail && 
                    u.Role == UserRole.Tenant &&
                    u.PasswordHash == passwordHash &&
                    u.Profile != null &&
                    u.RefreshToken == refresh
                ), 
                It.IsAny<CancellationToken>()),
            Times.Once);
        
        mocks.UnitOfWork.Verify(
            uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
        
        mocks.JwtProv.Verify(
            jwt => jwt.GenerateAccessToken(It.IsAny<User>()),
            Times.Once); 
    }
}

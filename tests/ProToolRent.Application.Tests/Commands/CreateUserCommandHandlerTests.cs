using FluentAssertions;
using Moq;
using ProToolRent.Application.Commands.CreateUser;
using ProToolRent.Application.Interfaces;
using ProToolRent.Domain.Entities;
using ProToolRent.Domain.Enums;
using ProToolRent.Domain.Interfaces;

namespace ProToolRent.Application.Tests;

public class CreateUserCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenDataIsValid_CreateUserObjectAndReturnsSuccess()
    {
        var mockUserRepo = new Mock<IUserRepository>();
        var mockPasswordHasher = new Mock<IPasswordHasher>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();

        mockPasswordHasher.Setup(ph => ph.Generate(It.IsAny<string>()))
            .Returns("hashPassword!");

        var handler = new CreateUserCommandHandler(
            mockUserRepo.Object,
            mockPasswordHasher.Object, 
            mockUnitOfWork.Object);
        var command = new CreateUserCommand(
            Email: "email@test.com",
            Password: "password",
            FirstName: "name",
            LastName: "last",
            City: "city",
            Organization: "org",
            Phone: "1234567",
            Role: UserRole.Tenant);
        
        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        mockPasswordHasher.Verify(
            ph => ph.Generate(It.IsAny<string>()), 
            Times.Once);

        mockUserRepo.Verify(
            repo => repo.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()),
            Times.Once);
        
        mockUnitOfWork.Verify(
            uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }
}

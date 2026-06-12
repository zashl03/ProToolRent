using FluentAssertions;
using Moq;
using ProToolRent.Application.Common;
using ProToolRent.Domain.Entities;
using ProToolRent.Domain.Enums;
using ProToolRent.Domain.Interfaces;

namespace ProToolRent.Application.Tests;

public class UpdateProfileCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenUserIsNull_ReturnsNotFound()
    {
        var mockUserRepo = new Mock<IUserRepository>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var userId = Guid.NewGuid();

        mockUserRepo.Setup(repo => repo.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User)null!);
        
        var handler = new UpdateProfileCommandHandler(mockUserRepo.Object,  mockUnitOfWork.Object);
        var command = new UpdateProfileCommand(
            UserId: userId,
            FirstName: "name",
            LastName: "last",
            City: "city",
            Organization: "org",
            Phone: "123456");
        
        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorType.NotFound);
        result.Error.Should().Be("User not found");

        mockUserRepo.Verify(
            repo => repo.GetByIdAsync(userId, It.IsAny<CancellationToken>()),
            Times.Once);

        mockUnitOfWork.Verify(
            uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenDataIsValid_UpdatesProfileAndReturnsSuccess()
    {
        var mockUserRepo = new Mock<IUserRepository>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var user = new User(email: "test@test.com", passwordHash: "pass", role: UserRole.Tenant);
        var profile = new UserProfile(
            firstName: "name",
            lastName: "last",
            city: "city",
            organization: "org",
            phone: "123456");
        user.SetProfile(profile);

        mockUserRepo.Setup(repo => repo.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        
        var handler = new UpdateProfileCommandHandler(mockUserRepo.Object,  mockUnitOfWork.Object);
        var command = new UpdateProfileCommand(
            UserId: user.Id,
            FirstName: "name2",
            LastName: "last2",
            City: "city2",
            Organization: "org2",
            Phone: "1234567");
        
        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        user.Profile.FirstName.Should().Be("name2");
        user.Profile.LastName.Should().Be("last2");
        user.Profile.City.Should().Be("city2");
        user.Profile.Organization.Should().Be("org2");
        user.Profile.Phone.Should().Be("1234567");

        mockUserRepo.Verify(
            repo => repo.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()),
            Times.Once);

        mockUnitOfWork.Verify(
            uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }
}

using ProToolRent.Domain.Entities;
using ProToolRent.Domain.Enums;

namespace ProToolRent.Domain.tests;

public class UserTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WhenEmailIsInvalid_ThrowsException(string? invalidEmail)
    {
        var ex = Assert.Throws<ArgumentException>(() => 
            new User(email: invalidEmail!, passwordHash: "pass", role: UserRole.Tenant));
        
        Assert.Equal("email", ex.ParamName);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WhenPasswordHashIsInvalid_ThrowsException(string? invalidPasswordHash)
    {
        var ex = Assert.Throws<ArgumentException>(() => 
            new User(email: "email", passwordHash: invalidPasswordHash!, role: UserRole.Tenant));
        
        Assert.Equal("passwordHash", ex.ParamName);
    }
    
    [Fact]
    public void Constructor_WhenDataIsValid_CreatesUserObject()
    {
        var user = new User(email: "email", passwordHash: "pass", role: UserRole.Tenant);

        Assert.Equal("email", user.Email);
        Assert.Equal("pass", user.PasswordHash);
        Assert.Equal(UserRole.Tenant, user.Role);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdatePasswordHash_WhenNewPasswordHashIsInvalid_ThrowsException(string? invalindNewPasswordHash)
    {
        var user = new User(email: "email", passwordHash: "pass", role: UserRole.Tenant);
        var ex = Assert.Throws<ArgumentException>(() => 
            user.UpdatePasswordHash(invalindNewPasswordHash!));
        
        Assert.Equal("newPasswordHash", ex.ParamName);
    }

    [Fact]
    public void UpdatePasswordHash_UpdatesPassword()
    {
        var user = new User(email: "email", passwordHash: "pass", role: UserRole.Tenant);

        user.UpdatePasswordHash("newhash");

        Assert.Equal("newhash", user.PasswordHash);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void SetRefreshToken_WhenRefreshTokenIsInvalid_ThrowsException(string? invalidRefreshToken)
    {
        var user = new User(email: "email", passwordHash: "pass", role: UserRole.Tenant);
        var ex = Assert.Throws<ArgumentException>(() => 
            user.SetRefreshToken(invalidRefreshToken!, DateTime.UtcNow.AddDays(7)));
        
        Assert.Equal("refreshToken", ex.ParamName);
    }
    
    [Fact]
    public void SetRefreshToken_WhenDateInThePast_ThrowsException()
    {
        var user = new User(email: "email", passwordHash: "pass", role: UserRole.Tenant);
        var ex = Assert.Throws<ArgumentException>(() => 
            user.SetRefreshToken("refresh", DateTime.UtcNow.AddDays(-5)));

        Assert.Equal("expiresAt", ex.ParamName);
    }

    [Fact]
    public void SetProfile_WhenDataIsValid_SetsUserProfile()
    {
        var user = new User(email: "email", passwordHash: "pass", role: UserRole.Tenant);
        var userProfile = new UserProfile(
            firstName: "name",
            lastName: "lastname",
            city: "city",
            organization: "organization",
            phone: "123456789"
        );

        user.SetProfile(userProfile);

        Assert.Equal(userProfile, user.Profile);
        Assert.Equal(user, userProfile.User);
    }

    [Fact]
    public void ResetRefreshToken_ClearsRefreshTokenAndExpiration()
    {
        var user = new User(email: "email", passwordHash: "pass", role: UserRole.Tenant);
        user.SetRefreshToken("refresh", DateTime.UtcNow.AddDays(7));
        
        user.ResetRefreshToken();
        
        Assert.Null(user.RefreshToken);
        Assert.Null(user.RefreshTokenExpiresAt);
    }
}

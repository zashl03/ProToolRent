using System.IO.Compression;
using ProToolRent.Domain.Enums;

namespace ProToolRent.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string? RefreshToken { get; private set; }
    public DateTime? RefreshTokenExpiresAt { get; private set; }
    public UserRole Role { get; private set; }
    public Guid ProfileId { get; private set; }
    public UserProfile Profile { get; private set; } = null!;

    private User() { }
    public User(string email, string passwordHash, UserRole role)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email of user is required", nameof(email));
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("PasswordHash of user is required", nameof(passwordHash));

        Email = email;
        PasswordHash = passwordHash;
        Role = role;
    }

    public void UpdateUser(string firstName, string lastName, string city, string organization, string phone)
    {
        Profile.UpdateProfile(firstName, lastName, city, organization, phone);
    }

    public void UpdatePasswordHash(string newPasswordHash)
    {
        if(string.IsNullOrWhiteSpace(newPasswordHash))
            throw new ArgumentException("New password is required", nameof(newPasswordHash));
        PasswordHash = newPasswordHash;
    }

    public void SetRefreshToken(string refreshToken, DateTime expiresAt)
    {
        if(string.IsNullOrWhiteSpace(refreshToken))
            throw new ArgumentException("Refresh token is required", nameof(refreshToken));
        if(expiresAt <= DateTime.UtcNow)
            throw new ArgumentException("Expiration must be in the future", nameof(expiresAt));

        RefreshToken = refreshToken;
        RefreshTokenExpiresAt = expiresAt;
    }

    public void ResetRefreshToken()
    {
        RefreshToken = null;
        RefreshTokenExpiresAt = null;
    }

    public void SetProfile(UserProfile profile)
    {
        Profile = profile;
        ProfileId = profile.Id;
        Profile.SetUser(this);
    }
}

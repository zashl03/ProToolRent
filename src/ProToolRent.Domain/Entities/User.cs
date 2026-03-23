namespace ProToolRent.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public Guid RoleId { get; private set; }

    private User() { }
    public User(string email, string passwordHash, Guid roleId)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email of user is required", nameof(email));
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("PasswordHash of user is required", nameof(passwordHash));

        Email = email;
        PasswordHash = passwordHash;
        RoleId = roleId;
    }
}

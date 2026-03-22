namespace ProToolRent.Domain.Entities;

public class UserProfile
{
    public Guid Id { get; private set; }
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set;} = string.Empty;
    public string City { get; private set; } = string.Empty;
    public string Organization { get; private set; } = string.Empty;
    public string Phone { get; private set; } = string.Empty;
    public Guid UserId { get; private set; }

    private UserProfile() { }
    public UserProfile(string firstName, string lastName, string city, string organization, string phone, Guid userId)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("FirstName of user is required", nameof(firstName));
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("LastName of user is required", nameof(lastName));
        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException("City of user is required", nameof(city));
        if (string.IsNullOrWhiteSpace(organization))
            throw new ArgumentException("Organization of user is required", nameof(organization));
        if (string.IsNullOrWhiteSpace(phone))
            throw new ArgumentException("Phone of user is required", nameof(phone));

        FirstName = firstName;
        LastName = lastName;
        City = city;
        Organization = organization;
        Phone = phone;
        UserId = userId;
    }
}

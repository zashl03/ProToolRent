
namespace ProToolRent.Domain.Entities
{
    public class User
    {
        public Guid Id { get; private set; }
        public string Fullname { get; private set; } = string.Empty;
        public string Organization { get; private set; } = string.Empty;
        public string City { get; private set; } = string.Empty;
        public Guid RoleId { get; private set; }
        

        public User(string fullname, string organization, string city, Guid roleId)
        {
            if (string.IsNullOrWhiteSpace(fullname))
                throw new ArgumentException("Fullname of user is required", nameof(fullname));
            if (string.IsNullOrWhiteSpace(organization))
                throw new ArgumentException("Organization of user is required", nameof(organization));
            if (string.IsNullOrWhiteSpace(city))
                throw new ArgumentException("City of user is required", nameof(city));
            Fullname = fullname;
            Organization = organization;
            City = city;
            RoleId = roleId;
        }
    }
}

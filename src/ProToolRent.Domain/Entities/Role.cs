
namespace ProToolRent.Domain.Entities
{
    public class Role
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = string.Empty;

        public Role(string name)
        {
            if(string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name of role is required", nameof(name)); 
            Name = name;
        }
    }
}

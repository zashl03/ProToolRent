
namespace ProToolRent.Domain.ValueObjects
{
    public readonly record struct Specification
    {
        public string Brand { get; }
        public string Name { get; }
        public double Power { get; }

        public Specification(string brand, string name, double power)
        {
            if(string.IsNullOrWhiteSpace(brand))
                throw new ArgumentException("Brand of tool is required", nameof(brand));
            if(string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name of tool is required", nameof(name));
            if(power < 0) 
                throw new ArgumentException("Power must be above 0", nameof(power));

            Brand = brand;
            Name = name;
            Power = power;
        }
    }
}

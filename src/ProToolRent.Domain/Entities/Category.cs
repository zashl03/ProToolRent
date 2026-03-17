
namespace ProToolRent.Domain.Entities
{
    public class Category
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public Guid? ParentId { get; private set; }
        public Category? Parent { get; private set; }

        private Category () { }

        public Category(string name, Guid? parentId = null, Category? parent = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name of category is required", nameof(name));
            Name = name;
            ParentId = parentId;
            Parent = parent;
        }
    }
}

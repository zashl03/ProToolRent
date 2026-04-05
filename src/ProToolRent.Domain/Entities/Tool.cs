using ProToolRent.Domain.ValueObjects;

namespace ProToolRent.Domain.Entities;

public class Tool
{
    public Guid Id { get; private set; }
    public Specification Specification { get; private set; }
    public Quantity Quantity { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public Guid CategoryId { get; private set; }
    public Guid UserId { get; private set; }
    public string ImageUrl {get; private set; }

    private Tool() { }

    public Tool(Specification specification,
        Quantity quantity,
        string description, 
        decimal price, 
        Guid categoryId, 
        Guid userId)
    {
        if (price < 0) 
            throw new ArgumentException("Price must be above 0", nameof(price));
        Specification = specification;
        Description = description;
        Quantity = quantity;
        Price = price;
        CategoryId = categoryId;
        UserId = userId;
    }

    public void UploadImage(string imageUrl)
    {
        ImageUrl = imageUrl;
    }
}

namespace ProToolRent.Domain.Entities;

public class Order
{
    public Guid Id { get; private set; }
    public string Status { get; private set; } = string.Empty;
    public DateTime CreatedDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public Guid UserId { get; private set; }
    public ICollection<OrderItem> OrderItems { get; private set; } = new List<OrderItem>();

    public Order(Guid userId)
    {
        Status = "Создан";
        CreatedDate = DateTime.UtcNow;
        UserId = userId; 
    }

    public bool RemoveItem(Guid id)
    {
        var item = OrderItems.FirstOrDefault(x => x.Id == id);

        if (item == null)
            return false;

        OrderItems.Remove(item);
        return true;
    }

    public Guid AddItem(decimal cost, int quantity, Tool tool)
    {
        var item = new OrderItem(cost, quantity, tool);

        OrderItems.Add(item);

        return item.Id;
    }
}

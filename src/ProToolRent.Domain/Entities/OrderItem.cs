
namespace ProToolRent.Domain.Entities;

public class OrderItem
{
    public Guid Id { get; private set; }
    public DateTime CreatedDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public decimal Cost { get; private set; }
    public int Quantity { get; private set; }

    public Guid ToolId { get; private set; }
    public Tool Tool { get; private set; } = null!;
    public Guid OrderId { get; private set; }
    public Order Order { get; private set; } = null!;

    private OrderItem() { }

    public OrderItem(decimal cost, int quantity, Tool tool)
    {
        if (cost < 0)
            throw new ArgumentException("Cost of orderitem must be more than 0", nameof(cost));

        if (quantity < 0)
            throw new ArgumentException("Quantity of orderitem must be more than 0", nameof(quantity));

        CreatedDate = DateTime.Now;
        Cost = cost;
        Quantity = quantity;
        Tool = tool;
    }
}

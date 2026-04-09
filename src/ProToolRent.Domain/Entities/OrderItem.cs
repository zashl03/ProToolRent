using Microsoft.VisualBasic;

namespace ProToolRent.Domain.Entities;

public class OrderItem
{
    public Guid Id { get; private set; }
    public DateOnly CreatedDate { get; private set; }
    public DateOnly EndDate { get; private set; }
    public decimal Cost { get; private set; }
    public int Quantity { get; private set; }

    public Guid ToolId { get; private set; }
    public Tool Tool { get; private set; } = null!;
    public Guid OrderId { get; private set; }
    public Order Order { get; private set; } = null!;

    private OrderItem() { }

    public OrderItem(DateOnly createdDate, DateOnly endDate, int quantity, Tool tool)
    {
        if (quantity < 0)
            throw new ArgumentException("Quantity of orderitem must be more than 0", nameof(quantity));
        if(endDate < createdDate)
            throw new ArgumentException("The end date must be later than the start date", nameof(createdDate));
        if(quantity > tool.Quantity.Available)
            throw new ArgumentException("This tool is booked", nameof(quantity));

        CreatedDate = createdDate;
        EndDate = endDate;
        Cost = (EndDate.DayNumber - CreatedDate.DayNumber) * tool.Price;
        Quantity = quantity;
        Tool = tool;
        Tool.ReserveQuantity(quantity);
    }
}

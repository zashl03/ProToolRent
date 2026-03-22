
namespace ProToolRent.Domain.Entities;

public class Order
{
    public Guid Id { get; private set; }
    public string Status { get; private set; } = string.Empty;
    public DateTime CreatedDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public ICollection<OrderItem> OrderItems { get; private set; } = new List<OrderItem>();
    private Order() { }

    public Order(string status, DateTime createdDate, DateTime endDate)
    {
        if(string.IsNullOrWhiteSpace(status))
            throw new ArgumentException("Status of order is required", nameof(status));

        Status = status;
        CreatedDate = createdDate;
        EndDate = endDate;
    }
}

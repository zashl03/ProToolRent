namespace ProToolRent.Api.Contracts.Requests;

public record CreateOrderItemRequest
(
    Guid OrderId,
    Guid ToolId,
    DateOnly StartDate,
    DateOnly EndDate,
    int Quantity
);

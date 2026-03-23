namespace ProToolRent.Api.Contracts.Requests;

public record CreateOrderItemRequest
(
    decimal Cost,
    int Quantity,
    Guid OrderId,
    Guid ToolId
);

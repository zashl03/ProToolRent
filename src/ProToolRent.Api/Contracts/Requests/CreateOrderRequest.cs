namespace ProToolRent.Api.Contracts.Requests;

public record CreateOrderRequest
(
    Guid UserId,
    Guid ToolId,
    DateOnly StartDate,
    DateOnly EndDate,
    int Quantity
);

namespace ProToolRent.Api.Contracts.Requests;

public record CreateToolRequest
(
    string Brand,
    string Name,
    double Power,
    string Description,
    int TotalQuantity,
    int ReservedQuantity,
    decimal Price,
    Guid CategoryId,
    Guid UserId
);

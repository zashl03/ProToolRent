namespace ProToolRent.Api.Contracts.Requests;

public record CreateToolRequest
(
    string Brand,
    string Name,
    double Power,
    string Description,
    int TotalQuantity,
    decimal Price,
    Guid CategoryId
);

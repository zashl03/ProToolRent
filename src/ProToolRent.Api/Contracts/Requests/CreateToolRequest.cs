namespace ProToolRent.Api.Contracts.Requests;

public record CreateToolRequest
(
    string Brand,
    string Name,
    double Power,
    string Description,
    string Status,
    double Price,
    Guid CategoryId,
    Guid UserId
);

using MediatR;
using ProToolRent.Application.Common;
using ProToolRent.Application.DTOs;

namespace ProToolRent.Application.Commands.CreateTool;

public record CreateToolCommand
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
) : IRequest<Result<Guid>>;

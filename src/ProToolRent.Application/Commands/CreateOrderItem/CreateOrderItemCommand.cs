using MediatR;
using ProToolRent.Application.Common;

namespace ProToolRent.Application.Commands.CreateOrderItem;

public record CreateOrderItemCommand
(
    Guid OrderId,
    Guid ToolId,
    DateOnly StartDate,
    DateOnly EndDate,
    int Quantity
) : IRequest<Result<Guid>>;

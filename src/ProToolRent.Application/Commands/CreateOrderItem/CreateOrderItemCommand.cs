using MediatR;
using ProToolRent.Application.Common;

namespace ProToolRent.Application.Commands.CreateOrderItem;

public record CreateOrderItemCommand
(
    decimal Cost,
    int Quantity,
    Guid OrderId,
    Guid ToolId
) : IRequest<Result<Guid>>;

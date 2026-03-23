using MediatR;
using ProToolRent.Application.Common;

namespace ProToolRent.Application.Commands.DeleteOrderItem;

public record DeleteOrderItemCommand
(
    Guid OrderId, 
    Guid OrderItemId
) : IRequest<Result>;

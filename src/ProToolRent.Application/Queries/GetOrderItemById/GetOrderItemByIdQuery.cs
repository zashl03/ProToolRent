using MediatR;
using ProToolRent.Application.Common;
using ProToolRent.Application.DTOs;

namespace ProToolRent.Application.Queries.GetOrderItemById;

public record GetOrderItemByIdQuery
(
    Guid OrderId,
    Guid OrderItemId
) : IRequest<Result<OrderItemDto>>;

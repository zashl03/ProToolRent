using MediatR;
using ProToolRent.Application.Common;
using ProToolRent.Application.DTOs;

namespace ProToolRent.Application.Queries.GetOrders;

public record GetOrdersQuery
(
    Guid UserId
): IRequest<Result<List<OrderSummaryDto>>>;
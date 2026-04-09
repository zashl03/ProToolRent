using MediatR;
using ProToolRent.Application.Common;
using ProToolRent.Application.DTOs;

namespace ProToolRent.Application.Commands.CreateOrder;

public record CreateOrderCommand
(
    Guid UserId,
    Guid ToolId,
    DateOnly StartDate,
    DateOnly EndDate,
    int Quantity
) : IRequest<Result<OrderSummaryDto>>;

using MediatR;
using ProToolRent.Application.Common;

namespace ProToolRent.Application.Commands.CreateOrder;

public record CreateOrderCommand
(
    Guid UserProfileId
) : IRequest<Result<Guid>>;

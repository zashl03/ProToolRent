using MediatR;
using ProToolRent.Application.Common;
using ProToolRent.Application.DTOs;
using ProToolRent.Domain.Interfaces;

namespace ProToolRent.Application.Queries.GetOrderItemById;

public class GetOrderItemByIdQueryHandler : IRequestHandler<GetOrderItemByIdQuery, Result<OrderItemDto>>
{
    private readonly IOrderRepository _orderRepository;

    public GetOrderItemByIdQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<Result<OrderItemDto>> Handle(GetOrderItemByIdQuery request, CancellationToken ct)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId, ct);

        if(order == null)
        {
            return Result<OrderItemDto>.NotFound($"Order with {request.OrderId} not found");
        }

        var orderItem = order.OrderItems.FirstOrDefault(x => x.Id == request.OrderItemId);

        if(orderItem == null)
        {
            return Result<OrderItemDto>.NotFound($"Order item with {request.OrderItemId} not found");
        }

        var orderItemDto = new OrderItemDto(
            orderItem.Id, 
            orderItem.CreatedDate,
            orderItem.EndDate,
            orderItem.Cost,
            orderItem.Quantity,
            orderItem.ToolId);

        return Result<OrderItemDto>.Success(orderItemDto);
    }
}
